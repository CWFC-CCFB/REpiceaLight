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
    public class BasicBound
    {

        private bool isUpperBound;
        private Matrix value;


        protected BasicBound(bool IsUpperBound)
        {
            this.isUpperBound = IsUpperBound;
        }

        /// <summary>
        /// Set the value of the bound.
        /// </summary>
        /// <param name="value">a Matrix instance</param>
        internal virtual void SetBoundValue(Matrix value)
        {
            this.value = value;
        }

        public virtual Matrix GetBoundValue() { return value; }

        protected bool IsUpperBound() { return isUpperBound; }
    }

}
