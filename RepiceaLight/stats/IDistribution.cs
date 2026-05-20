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

namespace REpiceaLight.stats
{

    public enum DistributionType
    {
        GAUSSIAN,
        UNIFORM,
        NONPARAMETRIC,
        UNKNOWN,
        CHI_SQUARE,
        WISHART,
        STUDENT
    }

    public interface IDistribution : IMomentGettable
    {

        /// <summary>
        /// Return true if the distribution is parametric or false otherwise.
        /// </summary>
        /// <returns>a boolean</returns>
        bool IsParametric();

        /// <summary>
        /// Return true if the distribution is multivariate.
        /// </summary>
        /// <returns>a boolean</returns>
        bool IsMultivariate();

        /// <summary>
        /// Return true if the GaussianFunction instance is univariate.
        /// </summary>
        /// <returns>a boolean</returns>
        bool IsUnivariate();

        /// <summary>
        /// Provide the type of distribution.
        /// </summary>
        /// <returns>a DistributionType enum</returns>
        DistributionType GetDistributionType();


        /// <summary>
        /// Draw a random realization from the distribution.
        /// </summary>
        /// <returns>the realization embedded in a Matrix instance</returns>
        Matrix GetRandomRealization();


    }
}
