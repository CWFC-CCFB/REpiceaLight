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
using System;
using static REpiceaLight.stats.distributions.GaussianErrorTermList;

namespace REpiceaLight.stats.distributions
{
    public class GaussianErrorTerm : IComparable
    {

        internal readonly int distanceIndex;
        internal double value = double.NaN;
        internal readonly double normalizedValue;

        public GaussianErrorTerm(IIndexableErrorTerm caller) : this(caller, StatisticalUtility.GetRandom().NextGaussian())
        {
        }

        public GaussianErrorTerm(IIndexableErrorTerm caller, double normalizedValue)
        {
            this.distanceIndex = ((IIndexableErrorTerm)caller).GetErrorTermIndex();
            this.normalizedValue = normalizedValue;
        }


        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is GaussianErrorTerm))
                throw new ArgumentException("The object parameter should be an instance of GaussianErrorTerm class");

            GaussianErrorTerm errorTerm = (GaussianErrorTerm)obj;
            if (this.distanceIndex < errorTerm.distanceIndex)
                return -1;
            else if (this.distanceIndex == errorTerm.distanceIndex)
                return 0;
            else
                return 1;
        }


    }
}
