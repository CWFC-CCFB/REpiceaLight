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
using REpiceaLight.simulation.covariateproviders.treelevel;
using REpiceaLight.stats.distributions;
using System;

namespace REpiceaLight.simulation.hdrelationships
{
    /// <summary>
    /// A general interface for trees to be used in HD relationships.
    /// </summary>
    public interface IHDRelationshipTree : IMonteCarloSimulationCompliantObject,
                                                    GaussianErrorTermList.IIndexableErrorTerm,
                                                    IHeightMProvider {


        /// <summary>
        /// Provide the error group in case of different error correlation structure.
        /// For instance, if coniferous species have a correlation structure that 
        /// differs from that of broadleaved species.     
        /// /// </summary>
        /// <returns>an Enum that defines the group</returns>
        Enum GetHDRelationshipTreeErrorGroup();
    }

}
