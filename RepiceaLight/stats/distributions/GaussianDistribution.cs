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

namespace REpiceaLight.stats.distributions
{
    public class GaussianDistribution : StandardGaussianDistribution, IMomentSettable
    {

        /// <summary>
        /// Constructor. Creates a Gaussian distribution with mean mu and variance sigma2. NOTE: Matrix 
        /// sigma2 must be positive definite.
        /// </summary>
        /// <param name="mu">the mean of the function</param>
        /// <param name="sigma2">the variance of the function</param>
        public GaussianDistribution(Matrix mu, SymmetricMatrix sigma2)
        {
            SetMean(mu);
            SetVariance(sigma2);
        }

        /// <summary>
        /// Constructor for univariate Gaussian distribution.
        /// </summary>
        /// <param name="mean">the mean of the distribution</param>
        /// <param name="variance">the variance of the distribution</param>
        public GaussianDistribution(double mean, double variance)
        {
            Matrix mu = new Matrix(1, 1);
            mu.SetValueAt(0, 0, mean);
            SetMean(mu);
            SymmetricMatrix sigma2 = new SymmetricMatrix(1);
            sigma2.SetValueAt(0, 0, variance);
            SetVariance(sigma2);
        }


        /// <summary>
        /// Constructor for univariate Gaussian distribution centered on 0 with variance 1.
        /// </summary>
        public GaussianDistribution() : this(0d, 1d) {}

        public new void SetMean(Matrix mean) { base.SetMean(mean); }

        public new void SetVariance(SymmetricMatrix variance) { base.SetVariance(variance); }

    }

}
