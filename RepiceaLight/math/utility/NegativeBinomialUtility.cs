using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.math.utility
{
    public sealed class NegativeBinomialUtility
    {

        /**
         * The mass probability of a negative binomial distribution.<br>
         * <br>
         * It follows the SAS parameterization: <br>
         * <br>
         * Pr(y)= r(y + 1/theta)/(y! r(1/theta)) * (theta*mu)^y / (1+theta*mu)^(y + 1/theta)<br>
         * <br>
         * where r() stands for the Gamma function.
         *  
         * @see<a href=http://support.sas.com/documentation/cdl/en/statug/63033/HTML/default/viewer.htm#statug_genmod_sect030.htm> 
         * SAS online documentation </a>
         * @param y the count (must be equal to or greater than 0)
         * @param mean the mean of the distribution
         * @param dispersion the dispersion parameter
         * @return a mass probability
         */
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

        /**
         * Provide a quantile of the distribution.
         * @param cdfValue the cumulative mass
         * @param mean the mean of the distribution
         * @param dispersion the dispersion of the distribution
         * @return a quantile
         */
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
