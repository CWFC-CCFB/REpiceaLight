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
using REpiceaLight.stats;
using REpiceaLight.stats.estimates;
using System.Collections.Generic;

namespace REpiceaLight.simulation
{
    /// <summary>
    /// A specific class to handle model parameters in a module.
    /// </summary>
    public class ModelParameterEstimates : GaussianEstimate
    {
        protected readonly List<int> estimatedParameterIndices;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mean">a vector that corresponds to the mean value</param>
        /// <param name="variance">a symmetric positive definite matrix</param>
        public ModelParameterEstimates(Matrix mean, SymmetricMatrix variance) : base(mean, variance)
        {
            estimatedParameterIndices = new List<int>();
            SetEstimatedParameterIndices();
        }

        protected virtual void SetEstimatedParameterIndices()
        {
            for (int i = 0; i < GetMean().m_iRows; i++)
                estimatedParameterIndices.Add(i);
        }

        public List<int> GetTrueParameterIndices()
        {
            List<int> copyList = new List<int>();
            copyList.AddRange(estimatedParameterIndices);
            return copyList;
        }

        public override Matrix GetRandomDeviate()
        {
            Matrix lowerChol = GetDistribution().GetStandardDeviation();
            Matrix randomVector = StatisticalUtility.DrawRandomVector(lowerChol.m_iRows, DistributionType.GAUSSIAN);
            Matrix oMat = lowerChol.Multiply(randomVector);
            Matrix deviate = (Matrix) GetMean().Clone();
            deviate.AddElementsAt(estimatedParameterIndices, oMat);
            return deviate;
        }

    }
}
