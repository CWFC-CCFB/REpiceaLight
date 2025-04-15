/*
 * This file is part of the REpiceaLight project.
 *
 * Copyright (C) 2025 His Majesty the King in right of Canada
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
namespace REpiceaLight.math.utility
{
    public sealed class NegativeBinomialUtility
    {

        /// <summary>
        /// The mass probability of a negative binomial distribution.<br></br>
        /// It follows the SAS parameterization: <br></br>
        /// Pr(y)= r(y + 1/theta)/(y! r(1/theta)) * (theta* mu)^y / (1+theta* mu)^(y + 1/theta)<br></br>
        /// where r() stands for the Gamma function.<br></br>
        /// See <a href="http://support.sas.com/documentation/cdl/en/statug/63033/HTML/default/viewer.htm#statug_genmod_sect030.htm">SAS online documentation</a> 
        /// </summary>
        /// <param name="y">the count (must be equal to or greater than 0)</param>
        /// <param name="mean">the mean of the distribution</param>
        /// <param name="dispersion">the dispersion parameter</param>
        /// <returns>a mass probability</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double GetMassProbability(int y, double mean, double dispersion)
        {
            if (y < 0)
                throw new ArgumentException("The binomial distribution is designed for integer equals to or greater than 0!");
            else if (y == 0)
            {
                double dispersionTimesMean = dispersion * mean;
                double inverseDispersion = 1 / dispersion;
                double prob = 1d / (Math.Pow(1 + dispersionTimesMean, inverseDispersion));
                return prob;
            }
            else
            {
                double dispersionTimesMean = dispersion * mean;
                double inverseDispersion = 1 / dispersion;
                double prob = GammaUtility.Gamma(y + inverseDispersion) / (MathUtility.Factorial(y) * GammaUtility.Gamma(inverseDispersion))
                        * Math.Pow(dispersionTimesMean, y) / (Math.Pow(1 + dispersionTimesMean, y + inverseDispersion));
                return prob;
            }
        }

        /// <summary>
        /// Provide a quantile of the distribution.
        /// </summary>
        /// <param name="cdfValue">the cumulative mass</param>
        /// <param name="mean">the mean of the distribution</param>
        /// <param name="dispersion">the dispersion of the distribution</param>
        /// <returns>a quantile</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetQuantile(double cdfValue, double mean, double dispersion)
        {
            if (cdfValue < 0 || cdfValue > 1)
                throw new ArgumentException("The cdfValue parameter should be a double between 0 and 1!");
            double cumulativeMass = 0d;
            int y = 0;
            while (cumulativeMass < cdfValue)
                cumulativeMass += GetMassProbability(y++, mean, dispersion);
            return --y;
        }

    }
}
