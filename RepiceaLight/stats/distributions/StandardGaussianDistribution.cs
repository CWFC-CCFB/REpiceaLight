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
using REpiceaLight.math.utility;
using REpiceaLight.math;
using System;

namespace REpiceaLight.stats.distributions
{
    public class StandardGaussianDistribution : IContinuousDistribution
    {

        private static StandardGaussianDistribution Singleton;

        private Matrix mu;
        private SymmetricMatrix sigma2;
        private Matrix lowerCholTriangle;


        /// <summary>
        /// Constructor to create a Gaussian distribution with mean mu 0 and variance 1.
        /// </summary>
        internal StandardGaussianDistribution()
        {
            Matrix mu = new Matrix(1, 1);
            SetMean(mu);
            SymmetricMatrix sigma2 = new SymmetricMatrix(1);
            sigma2.SetValueAt(0, 0, 1d);
            SetVariance(sigma2);
        }

        protected virtual void SetMean(Matrix mu)
        {
            this.mu = mu;   
        }

        protected void SetVariance(SymmetricMatrix sigma2)
        {
            this.sigma2 = sigma2;
        }


        /// <summary>
        /// Returns the single instance of the StandardGaussianDistribution class.
        /// </summary>
        /// <returns>a StandardGaussianDistribution instance</returns>
        public static StandardGaussianDistribution GetInstance()
        {
            if (Singleton == null)
            {
                Singleton = new StandardGaussianDistribution();
            }
            return Singleton;
        }

        public bool IsMultivariate()
        {
            return GetMu().m_iRows > 1;
        }

        public Matrix GetRandomRealization()
        {
            Matrix mean = GetMean();
            Matrix standardDeviation = GetStandardDeviation();
            Matrix normalStandardDeviates = StatisticalUtility.DrawRandomVector(standardDeviation.m_iRows, DistributionType.GAUSSIAN);
            return mean.Add(standardDeviation.Multiply(normalStandardDeviates));
        }


        /// <summary>
        /// Provide the lower triangle of the Cholesky decomposition of the variance-covariance matrix.
        /// </summary>
        /// <returns>a Matrix</returns>
        public Matrix GetStandardDeviation()
        {
            if (lowerCholTriangle == null)
            {
                lowerCholTriangle = GetSigma2().GetLowerCholTriangle();
            }
            return lowerCholTriangle;
        }

        public Matrix GetMean() { return GetMu(); }

        public SymmetricMatrix GetVariance() { return GetSigma2(); }

        public DistributionType GetDistributionType() { return DistributionType.GAUSSIAN; }

        protected Matrix GetMu() { return mu; }

        protected SymmetricMatrix GetSigma2() { return (SymmetricMatrix)sigma2; }

        public bool IsParametric() { return true; }


        public bool IsUnivariate()
        {
            return !IsMultivariate();
        }


        /// <summary>
        /// Return the result of the probability density function of the distribution parameter.
        /// </summary>
        /// <param name="yValues">a single double value or a Matrix instance</param>
        /// <returns>a double</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public double GetProbabilityDensity(Matrix yValues)
        {
            if (yValues == null || !yValues.IsTheSameDimension(GetMu()))
                throw new InvalidOperationException("Vector y is either null or its dimensions are different from those of mu!");
            else
            {
                if (!IsMultivariate())
                {
                    double y = yValues.GetValueAt(0, 0);
                    double mu = GetMu().GetValueAt(0, 0);
                    double variance = GetSigma2().GetValueAt(0, 0);
                    return GaussianUtility.GetProbabilityDensity(y, mu, variance);
                }
                else
                {
                    int k = yValues.m_iRows;
                    Matrix residuals = yValues.Subtract(GetMu());
                    Matrix invSigma2 = GetSigma2().GetInverseMatrix();
                    return 1d / (Math.Pow(2 * Math.PI, 0.5 * k) * Math.Sqrt(GetSigma2().GetDeterminant())) * Math.Exp(-0.5 * residuals.Transpose().Multiply(invSigma2).Multiply(residuals).GetSumOfElements());
                }
            }
        }


    }

}
