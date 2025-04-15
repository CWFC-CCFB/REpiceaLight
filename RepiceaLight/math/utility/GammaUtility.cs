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
    /// A class that provides static methods related to the Gamma function.
    /// </summary>
    public sealed class GammaUtility
    {

        private static readonly double[] COEF = new double[]{1.000000000000000174663, 5716.400188274341379136, -14815.30426768413909044, 14291.49277657478554025,
            -6348.160217641458813289, 1301.608286058321874105, -108.1767053514369634679, 2.605696505611755827729, -0.7423452510201416151527e-2,
            0.5384136432509564062961e-7, -0.4023533141268236372067e-8};     // higher precision with these coefficients

        /// <summary>
        /// Compute the result of the Gamma function.<br></br>
        /// The implementation is the Lanczos approximation.
        /// </summary>
        /// <param name="z">the argument</param>
        /// <returns>the value of the function (double)</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Gamma(double z)
        {
            if (z <= 0d)
                throw new ArgumentException("The gamma function does not support null or negative values!");
            double result;

            if (z < 0.5)
                result = Math.PI / (Math.Sin(Math.PI * z) * Gamma(1 - z));
            else
            {
                z -= 1;
                double x = COEF[0];
                double pval;
                for (int k = 1; k < COEF.Length; k++)
                {
                    pval = COEF[k];
                    x += pval / (z + k);        // i+1 = k 
                }

                double t = z + COEF.Length - 1 - 0.5;
                result = Math.Sqrt(2d * Math.PI) * Math.Pow(t, (z + .5)) * Math.Exp(-t) * x;
            }
            return result;
        }

        /// <summary>
        /// Calculate the logarithm of the Gamma function.
        /// </summary>
        /// <param name="z">the argument</param>
        /// <returns>a double the logarithm of the Gamma function</returns>
        public static double LogGamma(double z)
        {
            return Math.Log(Gamma(z));
        }


        private static readonly double K = 1.461632;
        private static readonly double SQRT_TWICE_PI = Math.Sqrt(2 * Math.PI);
        private static readonly double C = SQRT_TWICE_PI / Math.Exp(1) - Gamma(K);

        /// <summary>
        /// Compute the approximation of the inverse Gamma function. <br></br>
        /// This approximation was designed by David W.Cantrell.
        /// </summary>
        /// <param name="z">a double equal to or greater than 1</param>
        /// <returns>a double</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double InverseGamma(double z)
        {
            if (z < 1)
                throw new ArgumentException("The method does not accept argument smaller than 1!");
            double l_x = Math.Log((z + C) / SQRT_TWICE_PI);
            double w_x = LambertW(l_x / Math.Exp(1));
            return l_x / w_x + .5;
        }


        /// <summary>
        /// Implements the Lambert W function.<br></br>
        /// Code taken from <a href="http://keithbriggs.info/software.html">http://keithbriggs.info/software.html</a>  
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static double LambertW(double z)
        {
            double eps = 4e-16;
            double em1 = 0.3678794411714423215955237701614608;
            double p, e, t, w;
            if (z < -em1 || double.IsInfinity(z) || Double.IsNaN(z))
                throw new ArgumentException("The parameter z must be greater than -0.367879!");
            if (z == 0d)
                return 0d;
            if (z < -em1 + 1e-4)
            { // series near -em1 in sqrt(q)
                double q = z + em1;
                double r = Math.Sqrt(q);
                double q2 = q * q;
                double q3 = q2 * q;
                return -1.0
                        + 2.331643981597124203363536062168 * r
                        - 1.812187885639363490240191647568 * q
                        + 1.936631114492359755363277457668 * r * q
                        - 2.353551201881614516821543561516 * q2
                        + 3.066858901050631912893148922704 * r * q2
                        - 4.175335600258177138854984177460 * q3
                        + 5.858023729874774148815053846119 * r * q3
                        - 8.401032217523977370984161688514 * q3 * q;  // error approx 1e-16
            }
            /* initial approx for iteration... */
            if (z < 1.0)
            { /* series near 0 */
                p = Math.Sqrt(2d * (2.7182818284590452353602874713526625 * z + 1.0));
                w = -1.0 + p * (1.0 + p * (-0.333333333333333333333 + p * 0.152777777777777777777777));
            }
            else
            {
                w = Math.Log(z); /* asymptotic */
            }
            if (z > 3.0)
            {
                w -= Math.Log(w); /* useful? */
            }
            for (int i = 0; i < 10; i++)
            { /* Halley iteration */
                e = Math.Exp(w);
                t = w * e - z;
                p = w + 1d;
                t /= e * p - 0.5 * (p + 1.0) * t / p;
                w -= t;
                if (Math.Abs(t) < eps * (1.0 + Math.Abs(w)))
                {
                    return w; /* rel-abs error */
                }
            }
            /* should never get here */
            throw new InvalidOperationException("Unable to reach convergence for z = " + z);
        }



        /// <summary>
        /// Compute the first derivative of the gamma function. <br></br>
        /// The calculation is based on the digamma function.<br></br> 
        /// See <a href="https://en.wikipedia.org/wiki/Digamma_function">Digamma function</a>
        /// </summary>
        /// <param name="d">the argument of the function</param>
        /// <returns>the first derivative (a double)</returns>
        public static double GammaFirstDerivative(double d)
        {
            return Gamma(d) * Digamma(d);
        }


        /// <summary>
        /// Compute an approximation of the digamma function. <br></br>
        /// The approximation is calculated as ln(d) - 1/2d. <br></br>
        /// See <a href="https://en.wikipedia.org/wiki/Digamma_function">Digamma function</a>
        /// </summary>
        /// <param name="d">a strictly positive double </param>
        /// <returns>a double</returns>
        public static double Digamma(double d)
        {
            if (d <= 0d)
                throw new ArgumentException("The digamma function is not defined for values smaller than or equal to 0!");

            double d_star = d;
            double corrTerm = 0;
            while (d_star < 6)
            {
                corrTerm += 1 / d_star;
                d_star += 1;
            }
            double result = GetDigammaExpansion(d_star) - corrTerm;
            return result;
        }


        private static double GetDigammaExpansion(double z)
        {
            double z2 = z * z;
            double z4 = z2 * z2;
            double z6 = z4 * z2;
            return Math.Log(z) - 1d / (2 * z) - 1d / (12 * z2) + 1d / (120 * z4) - 1d / (252 * z4 * z2) + 1d / (240 * z4 * z4) -
                    1d / (132 * z6 * z4) + 691d / (32760 * z6 * z6) - 1d / (12 * z6 * z6 * z2);
        }


        /// <summary>
        /// Compute an approximation of the trigamma function. <br></br>
        /// See <a href = "https://en.wikipedia.org/wiki/Trigamma_function">Trigamma function</a>
        /// </summary>
        /// <param name="d">a strictly positive double</param>
        /// <returns>a double</returns>
        public static double Trigamma(double d)
        {
            if (d <= 0d)
                throw new ArgumentException("The digamma function is not defined for values smaller than or equal to 0!");
            double d_star = d;
            double corrTerm = 0;
            while (d_star < 6)
            {
                corrTerm += -1 / (d_star * d_star);
                d_star += 1;
            }
            double expansion = GetTrigammaExpansion(d_star);
            double result = expansion - corrTerm;
            return result;
        }

        private static double GetTrigammaExpansion(double z)
        {
            double z2 = z * z;
            double z4 = z2 * z2;
            double z6 = z4 * z2;
            return 1d / z + 1d / (2 * z2) + 1d / (6 * z2 * z) - 1d / (30 * z4 * z) + 1d / (42 * z6 * z) - 1d / (30 * z6 * z2 * z) +
                    5d / (66 * z6 * z4 * z) - 691 / (2730 * z6 * z6 * z) + 7d / (6 * z6 * z6 * z2 * z);
        }

    }
}
