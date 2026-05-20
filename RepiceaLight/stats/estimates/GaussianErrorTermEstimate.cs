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
using static REpiceaLight.stats.StatisticalUtility;
using REpiceaLight.stats.distributions;

namespace REpiceaLight.stats.estimates
{
    public class GaussianErrorTermEstimate : AbstractEstimate
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="variance"> a positive double</param>
        /// <param name="correlationParameter">a double</param>
        /// <param name="type">a TypeMatrixR enum</param>
        public GaussianErrorTermEstimate(SymmetricMatrix variance, double correlationParameter, TypeMatrixR type) : base(new CenteredGaussianDistribution(variance, correlationParameter, type))
        {
            estimatorType = EstimatorType.LikelihoodBased;
        }

        /// <summary>
        /// Constructor for univariate distribution.
        /// </summary>
        /// <param name="variance">a positive double</param>
        public GaussianErrorTermEstimate(SymmetricMatrix variance) : base(new CenteredGaussianDistribution(variance))
        {
        }

        //public override IDistribution GetDistribution()
        //{
        //    return base.GetDistribution();
        //}


        public Matrix GetMean(GaussianErrorTermList errorTermList)
        {
            return ((CenteredGaussianDistribution) GetDistribution()).GetMean(errorTermList);
        }

        /// <summary>
        /// Provide the variance of the distribution given some error terms. The class adapts the 
        /// variance matrix as the number of error terms increases.
        /// </summary>
        /// <param name="errorTermList">a GaussianErrorTermList instance</param>
        /// <returns>a SymmetricMatrix instance</returns>
        public SymmetricMatrix GetVariance(GaussianErrorTermList errorTermList)
        {
            return ((CenteredGaussianDistribution) GetDistribution()).GetVariance(errorTermList);
        }

        public Matrix GetRandomDeviate(GaussianErrorTermList errorTermList)
        {
            return ((CenteredGaussianDistribution) GetDistribution()).GetRandomRealization(errorTermList);
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
