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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats
{
    public interface IMomentSettable
    {
        /// <summary>
        /// Set the mean of the distribution.
        /// </summary>
        /// <param name="m">a Matrix instance</param>
        void SetMean(Matrix m);

        /// <summary>
        /// Set the variance of the distribution.
        /// </summary>
        /// <param name="v">a SymmetricMatrix instance</param>
        void SetVariance(SymmetricMatrix v);
    }
}
