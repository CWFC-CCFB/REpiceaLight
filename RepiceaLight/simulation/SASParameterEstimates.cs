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

namespace REpiceaLight.simulation
{
    public class SASParameterEstimates : ModelParameterEstimates
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mean">a vector that corresponds to the mean value</param>
        /// <param name="variance">a symmetric positive definite matrix </param>
        public SASParameterEstimates(Matrix mean, SymmetricMatrix variance) : base(mean, variance)
        {
        }

        protected override void SetEstimatedParameterIndices()
        {
            Matrix mean = GetMean();
            for (int i = 0; i < mean.m_iRows; i++)
            {
                if (mean.GetValueAt(i, 0) != 0d && mean.GetValueAt(i, 0) != 1d)
                {
                    estimatedParameterIndices.Add(i);
                }
            }
            Matrix variance = GetVariance();
            if (variance != null && variance.m_iRows != estimatedParameterIndices.Count)
            {
                throw new ArgumentException("SASParameterEstimates: the variance matrix is not compatible with the vector of parameter estimates");
            }
        }

    }



}

