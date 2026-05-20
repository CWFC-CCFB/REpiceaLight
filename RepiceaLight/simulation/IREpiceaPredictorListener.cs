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
namespace REpiceaLight.simulation
{
    /// <summary>
    /// An interface for instances that can listen to an REpiceaPredictor object.
    /// </summary>
    public interface IREpiceaPredictorListener
    {
        /// <summary>
        /// A method to process events thrown by the REpiceaPredictor object.
        /// </summary>
        /// <param name="ev">an REpiceaPredictorEvent instance</param>
        void ModelBasedSimulatorDidThis(REpiceaPredictorEvent ev);

    }
}
