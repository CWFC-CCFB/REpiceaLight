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
using REpiceaLight.stats.distributions;
using System;
using System.Collections.Generic;

namespace REpiceaLight.stats.estimates
{

    public abstract class ResamplingBasedEstimate : AbstractEstimate, INumberOfRealizationsProvider
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dist">an AbstractEmpiricalDistribution instance</param>
        protected ResamplingBasedEstimate(AbstractEmpiricalDistribution dist) : base(dist)
        {
            estimatorType = EstimatorType.Resampling;
        }

        /// <summary>
        /// Add a realization to the empirical distribution. The method checks the conformity of the value argument,
        /// which must be a column vector to ensure a proper variance estimation.
        /// </summary>
        /// <param name="value">a Matrix instance</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddRealization(Matrix value)
        {
            if (CheckConformity(value))
                ((AbstractEmpiricalDistribution) GetDistribution()).AddRealization(value);
            else
                throw new InvalidOperationException("The matrix is not conform to previous observations!");
        }

        public override IDistribution GetDistribution()
        {
            return base.GetDistribution();
        }

        private bool CheckConformity(Matrix value)
        {
            if (value == null)
                throw new ArgumentException("The value argument must be a non null Matrix instance!");
            List<Matrix> observations = ((AbstractEmpiricalDistribution) GetDistribution()).GetRealizations();
            if (observations.Count == 0)
                return true;
            else
            {
                Matrix firstObservation = observations[0];
                return firstObservation.m_iRows == value.m_iRows && firstObservation.m_iCols == value.m_iCols;
            }
        }

        /// <summary>
        /// Provide the quantile associated to a particular probability.
        /// </summary>
        /// <param name="probability">the probability level</param>
        /// <returns>a Matrix instance that contains the quantiles</returns>
        internal abstract Matrix GetQuantileForProbability(double probability);

        public override ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha)
        {
            Matrix lowerBoundValue = GetQuantileForProbability(.5 * (1d - oneMinusAlpha));
            Matrix upperBoundValue = GetQuantileForProbability(1d - .5 * (1d - oneMinusAlpha));
            return new ConfidenceInterval(lowerBoundValue, upperBoundValue, oneMinusAlpha);
        }

        /// <summary>
        /// Provide the list of realizations in the empirical distribution.
        /// </summary>
        /// <returns>a List of Matrix instance</returns>
        public List<Matrix> GetRealizations() { return ((AbstractEmpiricalDistribution) GetDistribution()).GetRealizations(); }

        public int GetNumberOfRealizations() { return ((AbstractEmpiricalDistribution) GetDistribution()).GetNumberOfRealizations(); }

    }
}
