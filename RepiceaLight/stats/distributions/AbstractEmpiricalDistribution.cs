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
using System.Collections.Generic;

namespace REpiceaLight.stats.distributions
{
    public abstract class AbstractEmpiricalDistribution : IDistribution
    {


        protected readonly List<Matrix> observations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbstractEmpiricalDistribution()
        {
            observations = new List<Matrix>();
        }

        /// <summary>
        /// Provide the number of realizations in the distribution.
        /// </summary>
        /// <returns>an integer</returns>
        public int GetNumberOfRealizations() { return observations.Count; }

        /// <summary>
        /// Add a realization to this empirical distribution.
        /// </summary>
        /// <param name="value">a Matrix instance</param>
        public void AddRealization(Matrix value) { observations.Add(value); }

        /// <summary>
        /// Provide a list of the realizations contained in this empirical distribution.
        /// </summary>
        /// <returns>a List of Matrix instances</returns>
        public List<Matrix> GetRealizations() { return observations; }

        public bool IsParametric() { return false; }

        public bool IsMultivariate()
        {
            return observations != null && observations.Count > 0 ?
                observations[0] is Matrix && observations[0].m_iRows > 1 :
                    false;
        }

        public DistributionType GetDistributionType() { return DistributionType.NONPARAMETRIC; }

        public Matrix GetRandomRealization()
        {
            int observationIndex = (int)(StatisticalUtility.GetRandom().NextDouble() * GetNumberOfRealizations());
            return GetRealizations()[observationIndex];
        }

        public abstract Matrix GetMean();
        public abstract SymmetricMatrix GetVariance();

        public bool IsUnivariate() { return !IsMultivariate(); }
    }
}
