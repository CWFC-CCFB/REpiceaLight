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
namespace REpiceaLight.stats.estimates
{
    /// <summary>
    /// Ensure the instance can provide its number of realizations.
    /// </summary>
    public interface INumberOfRealizationsProvider
    {

        /// <summary>
        /// Provide the number of realizations the estimate is based on.
        /// </summary>
        /// <returns>an integer</returns>
        int GetNumberOfRealizations();
    }

}
