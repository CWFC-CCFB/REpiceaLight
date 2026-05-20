/*
 * This file is part of the REpiceaLight library.
 *
 * Copyright (C) 2026 His Majesty the King in right of Canada
 * Author: Mathieu Fortin, Canadian Forest Service
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed with the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE. See the GNU Lesser General Public
 * License for more details.
 *
 * Please see the license at http://www.gnu.org/copyleft/lesser.html.
 */
using REpiceaLight.math;
using REpiceaLight.math.utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Threading;

namespace REpiceaLight.stats
{
    /// <summary>
    /// A class with static methods applied to statistics.
    /// </summary>
    public sealed class StatisticalUtility
    {

        private static REpiceaRandom random;

        public enum TypeMatrixR
        {
            LINEAR,
            LINEAR_LOG,
            COMPOUND_SYMMETRY,
            POWER,
            ARMA,
            EXPONENTIAL
        }

        private static readonly Dictionary<TypeMatrixR, int> NbParmsMap = new Dictionary<TypeMatrixR, int>();

        static StatisticalUtility()
        {
            NbParmsMap[TypeMatrixR.LINEAR] = 2;
            NbParmsMap[TypeMatrixR.LINEAR_LOG] = 2;
            NbParmsMap[TypeMatrixR.COMPOUND_SYMMETRY] = 2;
            NbParmsMap[TypeMatrixR.POWER] = 2;
            NbParmsMap[TypeMatrixR.ARMA] = 3;
            NbParmsMap[TypeMatrixR.EXPONENTIAL] = 2;
        }

        /// <summary>
        ///  Provide a shortcut for inverting an AR1 correlation matrix.
        /// </summary>
        /// <param name="size">the size of the matrix</param>
        /// <param name="rho">the correlation between two successive observations</param>
        /// <returns>a Matrix instance</returns>
        public static Matrix GetInverseCorrelationAR1Matrix(int size, double rho)
        {
            if (size < 1)
                throw new ArgumentException("The size parameter must be equal to or greater than 1!");
            if (rho <= 0 || rho >= 1)
                throw new ArgumentException("The rho parameter must be greater than 0 and smaller than 1!");
            double rho2 = rho * rho;
            Matrix mat = new Matrix(size, size);
            for (int i = 0; i < mat.m_iRows; i++)
            {
                for (int j = i; j < mat.m_iCols; j++)
                {
                    if (j == i)
                    {
                        if (i == 0 || i == mat.m_iRows - 1)
                            mat.SetValueAt(i, j, 1d / (1d - rho2));
                        else
                            mat.SetValueAt(i, j, (1d + rho2) / (1d - rho2));
                    }
                    else if (j == i + 1)
                    {
                        mat.SetValueAt(i, j, -rho / (1d - rho2));
                        mat.SetValueAt(j, i, -rho / (1d - rho2));
                    }
                }
            }
            return mat;
        }

        
        public static SymmetricMatrix ConstructRMatrix(List<double> covParms, TypeMatrixR type, Matrix coordinates)
        {
            return ConstructRMatrix(covParms, type, new Matrix[]{ coordinates});
        }

        /// <summary>
        /// Compute the R matrix of the type set by the type argument.
        /// </summary>
        /// <param name="covParms">a List of double containing the parameters. The first is the variance parameter, 
        /// the second is the covariance parameter.In case of ARMA type, there is a third parameter which is the 
        /// gamma parameter.</param>
        /// <param name="type">a TypeMatrixR enum</param>
        /// <param name="coordinates">a series of Matrices instance that stand for the coordinates. These should
        /// be column vectors of the same size.Specifying two matrices implies that the Euclidean distance is based 
        /// on two dimensions. Three matrices means three dimensions and so on.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static SymmetricMatrix ConstructRMatrix(List<double> covParms, TypeMatrixR type, Matrix[] coordinates)
        {
            if (covParms == null || covParms.Count < NbParmsMap[type])
                throw new ArgumentException("The covParms list should contain this number of parameters: " + NbParmsMap[type] + " when using type " + type.ToString());
            if (coordinates == null || coordinates.Length == 0)
                throw new ArgumentException("The coordinates argument should contain at least one matrix.");
            int nrow = -1;
            // check if the coordinates argument complies
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (!coordinates[i].IsColumnVector())
                    throw new ArgumentException("The coordinates should contain only column vectors!");
                else
                {
                    if (nrow == -1)
                        nrow = coordinates[i].m_iRows;
                    else if (coordinates[i].m_iRows != nrow)
                        throw new ArgumentException("The coordinates should contain only column vectors of the same size!");
                }
            }

            double varianceParameter = covParms[0];
            double covarianceParameter = covParms[1];
            double gamma = type == TypeMatrixR.ARMA ? covParms[2] : 0;

            double distance;
            SymmetricMatrix matrixR = new SymmetricMatrix(nrow);
            for (int i = 0; i < nrow; i++)
            {
                for (int j = i; j < nrow; j++)
                {
                    double corr = 0d;
                    switch (type)
                    {
                        case TypeMatrixR.LINEAR:                    // linear case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            corr = 1 - covarianceParameter * distance;
                            if (corr >= 0)
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            break;
                        case TypeMatrixR.LINEAR_LOG:                // linear log case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            corr = distance == 0 ? 1d : 1 - covarianceParameter * Math.Log(distance);
                            if (corr >= 0)
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            break;
                        case TypeMatrixR.COMPOUND_SYMMETRY:
                            if (i == j)
                                matrixR.SetValueAt(i, j, varianceParameter + covarianceParameter);
                            else
                                matrixR.SetValueAt(i, j, covarianceParameter);
                            break;
                        case TypeMatrixR.POWER:                  // power case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            if (distance == 0)
                            {
                                corr = 1d;
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            else
                            {
                                corr = Math.Pow(covarianceParameter, distance);
                                if (corr >= 0)
                                    matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            break;
                        case TypeMatrixR.ARMA:
                            if (i == j)
                                matrixR.SetValueAt(i, i, varianceParameter);
                            else
                            {
                                distance = Math.Abs(i - j) - 1;
                                double powCol = Math.Pow(covarianceParameter, distance);
                                matrixR.SetValueAt(i, j, varianceParameter * gamma * powCol);
                            }
                            break;
                        case TypeMatrixR.EXPONENTIAL:
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            if (distance == 0)
                            {
                                corr = 1d;
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            else
                            {
                                corr = Math.Exp(-distance / covarianceParameter);
                                if (corr >= 0)
                                    matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Matrix.ConstructRMatrix() : This type of correlation structure is not supported in this function");
                    }
                }
            }
            return matrixR;
        }

        /// <summary>
        /// Generate a random vector.
        /// </summary>
        /// <param name="nrow"> the number of elements to be generated</param>
        /// <param name="type">the distribution type (a DistributionType enum variable)</param>
        /// <returns></returns>
        public static Matrix DrawRandomVector(int nrow, DistributionType type)
        {
            return StatisticalUtility.DrawRandomVector(nrow, type, StatisticalUtility.GetRandom());
        }


        /// <summary>
        /// Provide the singleton of the REpiceaRandom class.
        /// </summary>
        /// <returns>the REpiceaRandom singleton</returns>
        public static REpiceaRandom GetRandom()
        {
            if (random == null)
            {
                random = new REpiceaRandom();
            }
            return random;
        }

        /// <summary>
        /// Generate a random vector
        /// </summary>
        /// <param name="nrow">the number of elements to be generated</param>
        /// <param name="type"the distribution type (a DistributionType enum variable)></param>
        /// <param name="random">an REpiceaRandom instance</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Matrix DrawRandomVector(int nrow, DistributionType type, REpiceaRandom random)
        {
            Matrix matrix = new Matrix(nrow, 1);
            for (int i = 0; i < nrow; i++)
            {
                double number;
                switch (type)
                {
                    case DistributionType.GAUSSIAN:      // Gaussian random number ~ N(0,1)
                        number = random.NextGaussian();
                        break;
                    case DistributionType.UNIFORM:       // Uniform random number [0,1]
                        number = random.NextDouble();
                        break;
                    default:
                        throw new InvalidOperationException("Matrix.RandomVector() : The specified distribution is not supported in the function");
                }
                matrix.SetValueAt(i, 0, number);
            }
            return matrix;
        }

        /// <summary>
        /// Provide the number of combinations
        /// </summary>
        /// <param name="n">the number of units</param>
        /// <param name="d">the number of units drawn in each combination</param>
        /// <returns>a long</returns>
        /// <exception cref="ArgumentException"></exception>
        public static long GetCombinations(int n, int d)
        {
            if (n < 1 || d < 1)
                throw new ArgumentException("Parameters n and d must be equal to or greater than 1!");
            else if (d > n)
                throw new ArgumentException("Parameters d must be equal to or smaller than parameter n!");
            double r = n - d > d ?
                 MathUtility.FactorialRatio(n, n - d) / MathUtility.Factorial(d) :
                    MathUtility.FactorialRatio(n, d) / MathUtility.Factorial(n - d);
            return (long)r; // TODO check if this cast works
        }


    }
}
