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
using System;
using System.Collections.Generic;
using static REpiceaLight.stats.StatisticalUtility;

namespace REpiceaLight.stats.distributions
{
    public sealed class CenteredGaussianDistribution : IDistribution
    {


        private readonly GaussianDistribution underlyingDistribution;
        private readonly double correlationParameter;
        private readonly TypeMatrixR? matrixType;
        private readonly bool isStructured;

        private readonly Dictionary<List<int>, SymmetricMatrix> structuredVarianceCovarianceMap;
        private readonly Dictionary<List<int>, Matrix> structuredLowerCholeskyMap;
        private readonly Dictionary<int, SymmetricMatrix> simpleVarianceCovarianceMap;
        private readonly Dictionary<int, Matrix> simpleLowerCholeskyMap;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="variance">the homogeneous variance</param>
        /// <param name="correlationParameter">the correlation parameter in the correlation structure</param>
        /// <param name="matrixType">a TypeMatrixR enum</param>
        /// <exception cref="ArgumentException"></exception>
        public CenteredGaussianDistribution(SymmetricMatrix variance, double correlationParameter, TypeMatrixR? matrixType)
        {
            underlyingDistribution = new GaussianDistribution(new Matrix(variance.m_iRows, 1), variance);
            this.correlationParameter = correlationParameter;
            this.matrixType = matrixType;
            isStructured = this.correlationParameter != 0 && this.matrixType != null;
            if (isStructured && variance.m_iRows > 1)
                throw new ArgumentException("The CenteredGaussianDistribution is not designed for a multivariate distribution with heterogeneous variances yet.");
            structuredVarianceCovarianceMap = new Dictionary<List<int>, SymmetricMatrix>();
            structuredLowerCholeskyMap = new Dictionary<List<int>, Matrix>();
            simpleVarianceCovarianceMap = new Dictionary<int, SymmetricMatrix>();
            simpleLowerCholeskyMap = new Dictionary<int, Matrix>();
        }

        /// <summary>
        /// Constructor without correlation structure.
        /// </summary>
        /// <param name="variance">the homogeneous variance</param>
        public CenteredGaussianDistribution(SymmetricMatrix variance) : this(variance, 0d, null) {}

        private Matrix GetLowerCholesky(List<int> indexList)
        {
            if (IsStructured())
            {
                if (!structuredLowerCholeskyMap.ContainsKey(indexList))
                    UpdateMaps(indexList);
                return structuredLowerCholeskyMap[indexList];
            }
            else
            {
                int size = indexList.Count;
                if (!simpleLowerCholeskyMap.ContainsKey(size))
                    UpdateMaps(size);

                return simpleLowerCholeskyMap[size];
            }
        }

        private void UpdateMaps(object key)
        {
            if (key is List<int>)
            {
                List<int> referenceList = new List<int>();
                referenceList.AddRange((List<int>)key);      // make a copy to avoid changes through reference
                Matrix distances = new Matrix(referenceList);
                if (!matrixType.HasValue)
                    throw new InvalidOperationException("The R Matrix has not been set!");
                SymmetricMatrix correlationMatrix = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, correlationParameter }, matrixType.Value, distances);
                SymmetricMatrix varianceCovariance = (SymmetricMatrix) correlationMatrix.ScalarMultiply(underlyingDistribution.GetVariance().GetValueAt(0, 0));
                structuredVarianceCovarianceMap[referenceList] = varianceCovariance;
                Matrix lowerChol = varianceCovariance.GetLowerCholTriangle();
                structuredLowerCholeskyMap[referenceList] = lowerChol;
            }
            else
            {
                int size = (int)key;
                DiagonalMatrix varianceCovariance = (DiagonalMatrix) Matrix.GetIdentityMatrix(size).ScalarMultiply(underlyingDistribution.GetVariance().GetValueAt(0, 0));
                simpleVarianceCovarianceMap[size] = varianceCovariance;
                Matrix lowerChol = varianceCovariance.GetLowerCholTriangle();
                simpleLowerCholeskyMap[size] = lowerChol;
            }
        }

        private SymmetricMatrix GetVariance(List<int> indexList)
        {
            if (IsStructured())
            {
                if (!structuredVarianceCovarianceMap.ContainsKey(indexList))
                    UpdateMaps(indexList);

                return structuredVarianceCovarianceMap[indexList];
            }
            else
            {
                int size = indexList.Count;
                if (!simpleVarianceCovarianceMap.ContainsKey(size))
                    UpdateMaps(size);

                return simpleVarianceCovarianceMap[size];
            }
        }

        public Matrix GetMean()
        {
            return underlyingDistribution.GetMean();
        }

        /// <summary>
        /// Should be used instead of getMean for structured variance.
        /// </summary>
        /// <param name="errorTermList">a GaussianErrorTermList instance</param>
        /// <returns>a Matrix instance</returns>
        /// <exception cref="ArgumentException"></exception>
        public Matrix GetMean(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
            Matrix chol = GetLowerCholesky(errorTermList.GetDistanceIndex());
            return chol.Multiply(errorTermList.GetNormalizedErrors());
        }

        public SymmetricMatrix GetVariance() { return underlyingDistribution.GetVariance(); }

        /// <summary>
        /// Provide the variance of the distribution given some error terms. The class adapts 
        /// the variance matrix as the number of error terms increases.
        /// </summary>
        /// <param name="errorTermList">a GaussianErrorTermList instance</param>
        /// <returns>SymmetricMatrix</returns>
        /// <exception cref="ArgumentException"></exception>
        public SymmetricMatrix GetVariance(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
  
            return GetVariance(errorTermList.GetDistanceIndex());
        }

        public Matrix GetRandomRealization() { return underlyingDistribution.GetRandomRealization(); }

        public Matrix GetRandomRealization(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
            Matrix errorTerms;
            if (!errorTermList.updated)
            {
                List<int> indexList = errorTermList.GetDistanceIndex();
                Matrix stdMat = GetLowerCholesky(indexList);
                Matrix normalizedErrorTerms = errorTermList.GetNormalizedErrors();
                errorTerms = stdMat.Multiply(normalizedErrorTerms);
                errorTermList.UpdateErrorTerm(errorTerms);
            }
            else
            {
                errorTerms = errorTermList.GetRealizedErrors();
            }
            return errorTerms;
        }
 


        public bool IsParametric() { return true; }

        public bool IsMultivariate() { return GetMean().m_iRows > 1; }

        public DistributionType GetDistributionType() { return DistributionType.GAUSSIAN; }

        public bool IsStructured() { return isStructured; }

        public bool IsUnivariate() { return !IsMultivariate(); }

    }

}
