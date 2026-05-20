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
using REpiceaLight.stats.estimates;

namespace REpiceaLight.stats
{
    public abstract class RandomVariable : IMomentGettable, IDistributionProvider
    {



        private readonly IDistribution distribution;

        protected RandomVariable(IDistribution distribution)
        {
            this.distribution = distribution;
        }

        /// <summary>
        /// Provide the distribution of the random variable.
        /// </summary>
        /// <returns>an IDistribution instance</returns>
        public virtual IDistribution GetDistribution() { return distribution; }

        public virtual Matrix GetMean() { return GetMeanFromDistribution(); }

        protected Matrix GetMeanFromDistribution() { return GetDistribution().GetMean(); }

        public virtual SymmetricMatrix GetVariance() { return GetVarianceFromDistribution(); }

        protected SymmetricMatrix GetVarianceFromDistribution() { return GetDistribution().GetVariance(); }

    }

}
