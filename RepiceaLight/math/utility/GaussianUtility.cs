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
    /// <summary>
    /// The Gaussian class implements common static methods that are related to the density or
    /// the cumulative probability of the Gaussian distribution.
    /// </summary>
    public class GaussianUtility
    {

        private static readonly double[] x = { 0.04691008, 0.23076534, 0.5, 0.76923466, 0.95308992 };
        private static readonly double[] w = { 0.018854042, 0.038088059, 0.0452707394, 0.038088059, 0.018854042 };

        /// <summary>
        /// Provide the cumulative probability of a bivariate standard normal distribution for quantiles x1 and x2.<br></br>
        /// The algorithm was translated from <a href="http://www.wilmott.com/pdfs/090721_west.pdf"> West, G. Better approximations to cumulative normal functions.  WILMOTT magazine. 70-76. </a> 
        /// </summary>
        /// <param name="x1">the first quantile</param>
        /// <param name="x2">the second quantile</param>
        /// <param name="rho">the correlation parameter</param>
        /// <returns>the joint probability</returns>
        public static double GetBivariateCumulativeProbability(double x1, double x2, double rho)
        {
            double h1, h2;
            double lh, h12;
            double h3, h5, h6, h7, h8;
            double r1, r2, r3, rr;
            double aa, ab;

            double bivarcumnorm = Double.NaN;

            h1 = x1;
            h2 = x2;
            h12 = (h1 * h1 + h2 * h2) * .5;
            if (Math.Abs(rho) >= .7)
            {
                r2 = 1 - rho * rho;
                r3 = Math.Sqrt(r2);
                if (rho < 0)
                {
                    h2 = -h2;
                }
                h3 = h1 * h2;
                h7 = Math.Exp(-h3 * .5);
                if (Math.Abs(rho) < 1)
                {
                    h6 = Math.Abs(h1 - h2);
                    h5 = h6 * h6 * .5;
                    h6 = h6 / r3;
                    aa = 0.5 - h3 * .125;
                    ab = 3 - 2 * aa * h5;
                    lh = 0.13298076 * h6 * ab * (1 - GetCumulativeProbability(h6)) - Math.Exp(-h5 / r2) * (ab + aa + r2) * .053051647;
                    lh = 0;
                    for (int i = 0; i < x.Length; i++)
                    {
                        r1 = r3 * x[i];
                        rr = r1 * r1;
                        r2 = Math.Sqrt(1 - rr);
                        if (h7 == 0)
                            h8 = 0;
                        else
                            h8 = Math.Exp(-h3 / (1 + r2)) / r2 / h7;
                        lh += lh - w[i] * Math.Exp(-h5 / rr) * (h8 - 1 - aa * rr);
                    }
                    bivarcumnorm = lh * r3 * h7 + GetCumulativeProbability(Math.Min(h1, h2));
                    if (rho < 0)
                        bivarcumnorm = GetCumulativeProbability(h1) - bivarcumnorm;
                }
            }
            else
            {
                h3 = h1 * h2;
                lh = 0;
                if (rho != 0d)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        r1 = rho * x[i];
                        r2 = 1 - r1 * r1;
                        lh += w[i] * Math.Exp((r1 * h3 - h12) / r2) / Math.Sqrt(r2);
                    }
                }
                bivarcumnorm = GetCumulativeProbability(h1) * GetCumulativeProbability(h2) + rho * lh;
            }
            return bivarcumnorm;
        }


        /// <summary>
        /// Provide the cumulative or complementary probability of a bivariate standard normal distribution for quantiles x1 and x2.<br></br>
        /// The algorithm was translated from <a href="http://www.wilmott.com/pdfs/090721_west.pdf"> West, G. Better approximations to cumulative normal functions.  WILMOTT magazine. 70-76. </a> 
        /// </summary>
        /// <param name="x1">the first quantile</param>
        /// <param name="x2">the second quantile</param>
        /// <param name="complementary1">a boolean true to obtain the complementary probability with respect to quantile x1 or false for the cumulative probability</param>
        /// <param name="complementary2">a boolean true to obtain the complementary probability with respect to quantile x2 or false for the cumulative probability</param>
        /// <param name="rho">the correlation parameter</param>
        /// <returns>the joint probability</returns>

        public static double GetBivariateCumulativeProbability(double x1, double x2, bool complementary1, bool complementary2, double rho)
        {
            if (complementary1)
                x1 = -x1;
            if (complementary2)
                x2 = -x2;

            if (complementary1 != complementary2)
                rho = -rho;

            return GaussianUtility.GetBivariateCumulativeProbability(x1, x2, rho);

        }

        /// <summary>
        /// Provide the cumulative probability of a standard normal distribution for quantile x.<br></br>
        /// The algorithm was translated from <a href="http://www.wilmott.com/pdfs/090721_west.pdf"> West, G. Better approximations to cumulative normal functions.  WILMOTT magazine. 70-76. </a> 
        /// </summary>
        /// <param name="x">the quantile</param>
        /// <returns>the probability</returns>
        public static double GetCumulativeProbability(double x)
        {
            double cumnorm = Double.NaN;
            double xAbs = Math.Abs(x);
            if (xAbs > 37)
            {
                cumnorm = 0;
            }
            else
            {
                double exp = Math.Exp(-xAbs * xAbs * .5);
                if (exp < 7.07106781186547)
                {
                    double build = 3.52624965998911E-2 * xAbs + 0.700383064443688;
                    build = build * xAbs + 6.37396220353165;
                    build = build * xAbs + 33.912866078383;
                    build = build * xAbs + 112.079291497871;
                    build = build * xAbs + 221.213596169931;
                    build = build * xAbs + 220.206867912376;
                    cumnorm = exp * build;
                    build = 8.83883476483184E-2 * xAbs + 1.75566716318264;
                    build = build * xAbs + 16.064177579207;
                    build = build * xAbs + 86.7807322029461;
                    build = build * xAbs + 296.564248779674;
                    build = build * xAbs + 637.333633378831;
                    build = build * xAbs + 793.826512519948;
                    build = build * xAbs + 440.413735824752;
                    cumnorm /= build;
                }
                else
                {
                    double build = xAbs + 0.65;
                    build = xAbs + 4d / build;
                    build = xAbs + 3d / build;
                    build = xAbs + 2d / build;
                    build = xAbs + 1d / build;
                    cumnorm = exp / build / 2.506628274631;
                }
            }
            if (x > 0)
            {
                cumnorm = 1 - cumnorm;
            }
            return cumnorm;
        }


        /// <summary>
        /// Provide the cumulative probability or the complementary probability of a standard normal distribution for quantile x.<br></br>
        /// The algorithm was translated from <a href="http://www.wilmott.com/pdfs/090721_west.pdf"> West, G. Better approximations to cumulative normal functions.  WILMOTT magazine. 70-76. </a> 
        /// </summary>
        /// <param name="x">the quantile</param>
        /// <param name="complementary">a boolean true to obtain the complementary probability or false to get the cumulative probability</param>
        /// <returns>the probability</returns>
        public static double GetCumulativeProbability(double x, bool complementary)
        {
            return complementary ?
                1d - GetCumulativeProbability(x) :
                    GetCumulativeProbability(x);
        }

        private static readonly double[] quantArrayA = new double[] { -3.969683028665376E+01, 2.209460984245205E+02, -2.759285104469687E+02, 1.383577518672690E+02, -3.066479806614716E+01, 2.506628277459239 };
        private static readonly double[] quantArrayB = new double[] { -5.447609879822406E+01, 1.615858368580409E+02, -1.556989798598866E+02, 6.680131188771972E+01, -1.328068155288572E+01 };
        private static readonly double[] quantArrayC = new double[] { -7.784894002430293E-03, -3.223964580411365E-01, -2.400758277161838, -2.549732539343734, 4.374664141464968, 2.938163982698783 };
        private static readonly double[] quantArrayD = new double[] { 7.784695709041462E-03, 3.224671290700398E-01, 2.445134137142996, 3.754408661907416 };

        /// <summary>
        /// Provide the quantile of the distribution
        /// </summary>
        /// <param name="cdfValue">the cumulative probability</param>
        /// <returns>the quantile</returns>
        public static double GetQuantile(double cdfValue)
        {

            double p_low = 0.02425;
            double p_high = 1 - p_low;
            double x;
            double q;

            if (cdfValue > 0 && cdfValue < p_low)
            {
                q = Math.Sqrt(-2d * Math.Log(cdfValue));
                x = (((((quantArrayC[0] * q + quantArrayC[1]) * q + quantArrayC[2]) * q + quantArrayC[3]) * q + quantArrayC[4]) * q + quantArrayC[5]) /
                        ((((quantArrayD[0] * q + quantArrayD[1]) * q + quantArrayD[2]) * q + quantArrayD[3]) * q + 1);
            }
            else if (cdfValue >= p_low && cdfValue <= p_high)
            {
                q = cdfValue - .5;
                double r = q * q;
                x = (((((quantArrayA[0] * r + quantArrayA[1]) * r + quantArrayA[2]) * r + quantArrayA[3]) * r + quantArrayA[4]) * r + quantArrayA[5]) * q /
                        (((((quantArrayB[0] * r + quantArrayB[1]) * r + quantArrayB[2]) * r + quantArrayB[3]) * r + quantArrayB[4]) * r + 1);
            }
            else if (cdfValue > p_high && cdfValue < 1)
            {
                q = Math.Sqrt(-2d * Math.Log(1 - cdfValue));
                x = -(((((quantArrayC[0] * q + quantArrayC[1]) * q + quantArrayC[2]) * q + quantArrayC[3]) * q + quantArrayC[4]) * q + quantArrayC[5]) /
                        ((((quantArrayD[0] * q + quantArrayD[1]) * q + quantArrayD[2]) * q + quantArrayD[3]) * q + 1);
            }
            else
            {
                throw new ArgumentException("The fValue parameter must be larger than 0 and smaller than 1!");
            }

            return IncreasePrecision(x, cdfValue);
        }

        /// <summary>
        /// Compute the probability density for a value of a normal distribution with mean mu and variance sigma2.
        /// </summary>
        /// <param name="y">the value</param>
        /// <param name="mu">the mean of the distribution</param>
        /// <param name="sigma2"> the variance of the distribution. Must be greater than 0.</param>
        /// <returns>a probability density</returns>
        public static double GetProbabilityDensity(double y, double mu, double sigma2)
        {
            if (sigma2 <= 0)
                throw new ArgumentException("The sigma2 parameter must be strictly positive (i.e. > 0)!");
            double diff = y - mu;
            return 1d / Math.Sqrt(2 * Math.PI * sigma2) *
                    Math.Exp(-0.5 * diff * diff / sigma2);
        }

        /// <summary>
        /// Compute the probability density for a quantile of the standard normal distribution. 
        /// </summary>
        /// <param name="y">the quantile</param>
        /// <returns>its probability density</returns>
        public static double GetProbabilityDensity(double y)
        {
            return GetProbabilityDensity(y, 0, 1);
        }

        private static double IncreasePrecision(double x, double cdfValue)
        {
            return x;
            //		double e = 0.5 * (1 - erf(-x/Math.sqrt(2d))) - cdfValue;
            //		double u = e * Math.sqrt(2d * Math.PI) * Math.exp(x*x/2d);
            //		return x - u / (1d + x * u *.5);
        }

    }

}
