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

namespace REpiceaLight.simulation
{
    /// <summary>
    /// An interface ensuring a stochastic implementation. 
    /// </summary>
    public interface IMonteCarloSimulationCompliantObject
    {

        /// <summary>
        /// Provide the id of the subject. This id remains constant throughout 
        /// the Monte Carlo iterations in case of stochastic implementation.
        /// </summary>
        /// <returns>a string that defines the subject id and that remains constant throughout the simulation </returns>
        String GetSubjectId();

        /// <summary>
        /// Provide the hierarchical level of the subject
        /// </summary>
        /// <returns>a Hierarchical instance</returns>
        HierarchicalLevel GetHierarchicalLevel();

                /// <summary>
        /// Provide the id of the Monte Carlo realization. It is mandatory for the implementation 
        /// of the random deviates on the parameter estimates.These deviates remain constant for 
        /// a particular Monte Carlo realization, regardless of the plot.
        /// </summary>
        /// <returns>an integer</returns>
        int GetMonteCarloRealizationId();

    }

}
