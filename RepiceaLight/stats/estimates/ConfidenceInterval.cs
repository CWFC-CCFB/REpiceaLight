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

namespace REpiceaLight.stats.estimates
{

    /// <summary>
    /// A class that implements prediction or confidence intervals
    /// </summary>
    public class ConfidenceInterval
    {

        /// <summary>
        /// An inner class that defines the bound of the interval.
        /// </summary>
        public class CIBound : BasicBound
        {

            internal CIBound(bool isUpperBound) : base(isUpperBound) { }

            //internal override void SetBoundValue(Matrix value)
            //{
            //    base.SetBoundValue(value);
            //}

            //public override Matrix GetBoundValue() { return base.GetBoundValue(); }
        }

        private readonly CIBound lowerBound;
        private readonly CIBound upperBound;
        private readonly double probabilityLevel;

         /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lowerBoundValue">a Matrix instance standing for the lower bound</param>
        /// <param name="upperBoundValue">a Matrix instance standing for the upper bound</param>
        /// <param name="probabilityLevel">the probability level associated with these bounds (e.g. 0.95)</param>
        public ConfidenceInterval(Matrix lowerBoundValue, Matrix upperBoundValue, double probabilityLevel)
        {
            lowerBound = new CIBound(false);
            upperBound = new CIBound(true);
            lowerBound.SetBoundValue(lowerBoundValue);
            upperBound.SetBoundValue(upperBoundValue);
            this.probabilityLevel = probabilityLevel;
        }

        /// <summary>
        /// Provide the lower bound of the interval.
        /// </summary>
        /// <returns>a Matrix instance</returns>
        public Matrix GetLowerLimit() { return lowerBound.GetBoundValue(); }

        /// <summary>
        /// Provide the upper bound of the interval.
        /// </summary>
        /// <returns>a Matrix instance</returns>
        public Matrix GetUpperLimit() { return upperBound.GetBoundValue(); }

        /// <summary>
        /// Provide the probability level of the interval.
        /// </summary>
        /// <returns>a double</returns>
        public double GetProbabilityLevel() { return probabilityLevel; }

        /// <summary>
        /// Check if any bound value is a not-a-number (NaN)
        /// </summary>
        /// <returns>a boolean</returns>
        public bool IsThereAnyNaN()
        {
            return GetLowerLimit().DoesContainAnyNaN() || GetUpperLimit().DoesContainAnyNaN();
        }

    }

}
