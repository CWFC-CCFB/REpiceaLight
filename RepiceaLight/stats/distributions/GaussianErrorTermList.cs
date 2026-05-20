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

namespace REpiceaLight.stats.distributions
{
    public sealed class GaussianErrorTermList : List<GaussianErrorTerm>
    {


        internal bool updated;

        /// <summary>
        /// Ensures the instance can return an index that will serve as distance for the calculation 
        /// of the variance-covariance matrix.
        /// </summary>
        public interface IIndexableErrorTerm
        {

            /// <summary>
            /// Provide the index of the error term. Typically, this value is the time and it serves to 
            /// calculate the distance between two observations when computing the variance-covariance matrix.
            /// </summary>
            /// <returns>an integer</returns>
            int GetErrorTermIndex();
        }

        public List<int> GetDistanceIndex()
        {
            List<int> indexList = new List<int>();
            foreach (GaussianErrorTerm res in this)
            {
                indexList.Add(res.distanceIndex);
            }
            return indexList;
        }

        public Matrix GetNormalizedErrors()
        {
            Matrix mat = new Matrix(Count, 1);
            for (int i = 0; i < Count; i++)
                mat.SetValueAt(i, 0, this[i].normalizedValue);

            return mat;
        }

        internal Matrix GetRealizedErrors()
        {
            Matrix mat = new Matrix(Count, 1);
            for (int i = 0; i < Count; i++)
                mat.SetValueAt(i, 0, this[i].value);

            return mat;
        }

        public void UpdateErrorTerm(Matrix errorTerms)
        {
            for (int i = 0; i < errorTerms.m_iRows; i++)
            {
                GaussianErrorTerm error = this[i];
                if (error.value == null)
                {
                    error.value = errorTerms.GetValueAt(i, 0);
                }
            }
            updated = true;
        }

        public double GetErrorForIndexableInstance(IIndexableErrorTerm indexableErrorTerm)
        {
            int distanceIndex = indexableErrorTerm.GetErrorTermIndex();
            int index = GetDistanceIndex().IndexOf(distanceIndex);
            if (index < 0)
                throw new ArgumentException("This distance index is not contained in the GaussianErrorTermList");
            else
                return this[index].value;
        }


        public void Add(GaussianErrorTerm term)
        {
            base.Add(term);
            updated = false;
            Sort();
        }

    }


}
