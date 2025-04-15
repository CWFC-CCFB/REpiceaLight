using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.math.utility
{
    public sealed class GammaUtility
    {

        private static readonly double[] COEF = new double[]{1.000000000000000174663, 5716.400188274341379136, -14815.30426768413909044, 14291.49277657478554025,
            -6348.160217641458813289, 1301.608286058321874105, -108.1767053514369634679, 2.605696505611755827729, -0.7423452510201416151527e-2,
            0.5384136432509564062961e-7, -0.4023533141268236372067e-8};     // higher precision with these coefficients

        /**
         * Compute the result of the Gamma function. <p>
         * The implementation is the Lanczos approximation. 
         * @param z the argument
         * @return a double the value of the function
         */
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

        /**
         * Calculate the logarithm of the Gamma function.
         * @param z the argument
         * @return a double the logarithm of the Gamma function
         */
        public static double logGamma(double z)
        {
            return Math.Log(Gamma(z));
        }


        private static readonly double K = 1.461632;
        private static readonly double SQRT_TWICE_PI = Math.Sqrt(2 * Math.PI);
        private static readonly double C = SQRT_TWICE_PI / Math.Exp(1) - Gamma(K);

        /**
         * Compute the approximation of the inverse Gamma function. <p>
         * This approximation was designed by David W. Cantrell. 
         * @param z a double equal or greater than 1.
         * @return a double
         * @see <a href="https://web.archive.org/web/20171104030158/http://mathforum.org/kb/message.jspa?messageID=342551&tstart=0">Link to David W. Cantrell post</a>
         */
        public static double InverseGamma(double z)
        {
            if (z < 1)
                throw new ArgumentException("The method does not accept argument smaller than 1!");
            double l_x = Math.Log((z + C) / SQRT_TWICE_PI);
            double w_x = LambertW(l_x / Math.Exp(1));
            return l_x / w_x + .5;
        }

        /**
         * This method implements the Lambert W function.
         * Code taken from http://keithbriggs.info/software.html  
         * @param z
         * @return
         */
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

        /**
         * Compute the first derivative of the gamma function. <p>
         * The calculation is based on the digamma function.
         * @see<a href=https://en.wikipedia.org/wiki/Digamma_function> Digamma function </a>
         * @param d the argument of the function
         * @return the first derivative (a double)
         */
        public static double GammaFirstDerivative(double d)
        {
            return Gamma(d) * Digamma(d);
        }


        /**
         * Compute an approximation of the digamma function. <br>
         * <br>
         * The approximation is calculated as ln(d) - 1/2d.
         * @see<a href=https://en.wikipedia.org/wiki/Digamma_function> Digamma function </a>
         * @param d a strictly positive double 
         * @return a double
         */
        public static double Digamma(double d)
        {
            if (d <= 0d)
            {
                throw new ArgumentException("The digamma function is not defined for values smaller than or equal to 0!");
            }
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


        /**
         * Compute an approximation of the digamma function. <br>
         * <br>
         * The approximation is calculated as ln(d) - 1/2d.
         * @see<a href=https://en.wikipedia.org/wiki/Digamma_function> Digamma function </a>
         * @param d a strictly positive double 
         * @return a double
         */
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
