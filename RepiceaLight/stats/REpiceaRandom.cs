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
using System.Drawing;

namespace REpiceaLight.stats
{
    public class REpiceaRandom : Random
    {


        private static readonly double OneThird = 1d / 3;

        internal REpiceaRandom() : base()
        {
        }

        public double NextGaussian()
        {
            return GaussianUtility.GetQuantile(NextDouble());
        }

        private double GetRandomGammaForShapeGreaterThanOrEqualToOne(double shape)
        {
            double d = shape - OneThird;
            double c = 1d / Math.Sqrt(9 * d);
            bool found = false;
            double z, u;
            double v = 0d;
            while (!found)
            {
                z = NextGaussian();
                u = NextDouble();
                v = Math.Pow(1 + c * z, 3d);
                bool firstCondition = z > -1d / c;
                bool secondCondition = Math.Log(u) < .5 * z * z + d - d * v + d * Math.Log(v);
                if (firstCondition)
                    if (secondCondition)
                        found = true;
            }
            return d * v;
        }

        private double GetRandomGammaForAnyShape(double shape)
        {
            if (shape >= 1)
                return GetRandomGammaForShapeGreaterThanOrEqualToOne(shape);
            else
            {
                double x = GetRandomGammaForShapeGreaterThanOrEqualToOne(shape + 1);
                return x * Math.Pow(NextDouble(), 1d / shape);
            }
        }

        /// <summary>
        /// Provide a random realization from a beta distribution with scales sacle1 and scale2.
        /// </summary>
        /// <param name="scale1">a double larger than 0</param>
        /// <param name="scale2">a double larger than 0</param>
        /// <returns>a double</returns>
        public double NextBeta(double scale1, double scale2)
        {
            double x = NextGamma(scale1, 1d);
            double y = NextGamma(scale2, 1d);
            return x / (x + y);
        }


        /// <summary>
        /// Provide a random realization from a Gamma distribution following Marsaglia and Tsang's method. The 
        /// mean of the distribution is obtained through the product of the shape and the scale.
        /// </summary>
        /// <param name="shape">a double larger than 0</param>
        /// <param name="scale">a double larger than 0</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double NextGamma(double shape, double scale)
        {
            if (shape <= 0d || scale <= 0d)
            {
                throw new ArgumentException("The shape and the scale must be larger than 0!");
            }
            double x = GetRandomGammaForAnyShape(shape);
            return x * scale;
        }


        /// <summary>
        /// Produce a random integer that follows negative binomial distribution.
        /// </summary>
        /// <param name="mean">the mean of the distribution</param>
        /// <param name="dispersion">the dispersion parameter</param>
        /// <returns>an integer</returns>
        public int NextNegativeBinomial(double mean, double dispersion)
        {
            double threshold = NextDouble();   
            double cumulativeProb = 0.0;
            int output = -1;

            while (threshold > cumulativeProb)
            {
                output++;
                double massProb = NegativeBinomialUtility.GetMassProbability(output, mean, dispersion);
                cumulativeProb += massProb;
            }
            return output;
        }


        /**
 * R
 * @param df 
 * @return 
 * @see <a href="https://doi.org/10.1090/S0025-5718-1994-1219702-8"> Bailey, R.W. 1994. Polar generation of random variances with the t-distribution. 
 * Mathematics of Computation 62(206): 779-781.</a>
 */


        /// <summary>
        /// Provide a random realization from the standard Student's t distribution. The algorithm is that of 
        /// Bailey(1994) based on polar generation.
        /// </summary>
        /// <param name="df">the degrees of freedom</param>
        /// <returns>a random deviate from the Student's t distribution</returns>    
        /// <see href="https://doi.org/10.1090/S0025-5718-1994-1219702-8"> Bailey, R.W. 1994. Polar generation 
        /// of random variances with the t-distribution. Mathematics of Computation 62(206): 779-781. </see>
        public double NextStudentT(double df)
        {
            double W = 2d;
            double U = 0;
            while (W > 1)
            {
                U = NextDouble();
                double V = NextDouble();
                U = 2 * U - 1;
                V = 2 * V - 1;
                W = U * U + V * V;
            }
            double C2 = U * U / W;
            double R2 = df * (Math.Pow(W, -2d / df) - 1);
            double result;
            if (NextDouble() < .5)
                result = Math.Sqrt(R2 * C2);
            else
                result = -Math.Sqrt(R2 * C2);
            return result;
        }


        /// <summary>
        /// Provide a random realization from a Chi square distribution.
        /// </summary>
        /// <param name="df">the degrees of freedom</param>
        /// <returns>a double</returns>
        /// <exception cref="ArgumentException"></exception>
        public double NextChiSquare(int df)
        {
            if (df <= 0)
                throw new ArgumentException("The number of degrees of freedom should be larger than 0");
            return NextGamma(df * .5, 2d);
        }

        /// <summary>
        /// Provide matrix A in the Bartlett decomposition.
        /// </summary>
        /// <param name="df">degrees of freedom</param>
        /// <param name="dim">the dimensions of the matrix</param>
        /// <returns>a Matrix</returns>
        public Matrix NextBartlettDecompositionMatrix(int df, int dim)
        {
            Matrix aMat = new Matrix(dim, dim);
            for (int i = 0; i < aMat.m_iRows; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                        aMat.SetValueAt(i, j, Math.Sqrt(NextChiSquare(df - i)));
                    else
                        aMat.SetValueAt(i, j, NextGaussian());
                }
            }
            return aMat;
        }

    }
}
