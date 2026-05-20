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
using REpiceaLight.stats.distributions;

namespace REpiceaLight.stats.estimates
{
    public class GaussianEstimate : AbstractEstimate, IMomentSettable
    {

        /// <summary>
        /// Common constructor. By default the Gaussian distribution that supports this estimate has a mean 0 and a variance 1.
        /// </summary>
        public GaussianEstimate() : base(new GaussianDistribution())
        {
            estimatorType = EstimatorType.LikelihoodBased;
        }

        /// <summary>
        /// Constructor with the mean and variance.
        /// </summary>
        /// <param name="mean">a Matrix instance that contains the mean </param>
        /// <param name="variance">a SymmetricMatrix instance that contains the variance-covariance</param>
        public GaussianEstimate(Matrix mean, SymmetricMatrix variance) : this()
        {
            SetMean(mean);
            SetVariance(variance);
        }

        /// <summary>
        /// Constructor for univariate distribution.
        /// </summary>
        /// <param name="mean">a double that stands for the mean</param>
        /// <param name="variance">a double that stands for the variance</param>
        public GaussianEstimate(double mean, double variance) : this()
        {
            Matrix meanMat = new Matrix(1, 1);
            meanMat.SetValueAt(0, 0, mean);
            SymmetricMatrix varianceMat = new SymmetricMatrix(1);
            varianceMat.SetValueAt(0, 0, variance);
            SetMean(meanMat);
            SetVariance(varianceMat);
        }


        public new GaussianDistribution GetDistribution()
        {
            return (GaussianDistribution)base.GetDistribution();
        }

        public void SetVariance(SymmetricMatrix variance)
        {
            GetDistribution().SetVariance(variance);
        }

        public void SetMean(Matrix mean)
        {
            GetDistribution().SetMean(mean);
        }

        protected Matrix GetQuantileForProbability(double probability)
        {
            Matrix stdDev = GetVariance().DiagonalVector().ElementWisePower(.5);
            double quantile = GaussianUtility.GetQuantile(probability);
            return GetMean().Add(stdDev.ScalarMultiply(quantile));
        }

        public override ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha)
        {
            Matrix lowerBoundValue = GetQuantileForProbability(.5 * (1d - oneMinusAlpha));
            Matrix upperBoundValue = GetQuantileForProbability(1d - .5 * (1d - oneMinusAlpha));
            return new ConfidenceInterval(lowerBoundValue, upperBoundValue, oneMinusAlpha);
        }

    }

}
