using REpiceaLight.math;
using REpiceaLight.math.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats
{
    public class REpiceaRandom : Random
    {


        private static readonly double OneThird = 1d / 3;

        internal REpiceaRandom() : base()
        {
        }

        public double NextGaussian()
        {
            return GaussianUtility.GetQuantile(NextDouble());
        }

        private double GetRandomGammaForShapeGreaterThanOrEqualToOne(double shape)
        {
            double d = shape - OneThird;
            double c = 1d / Math.Sqrt(9 * d);
            bool found = false;
            double z, u;
            double v = 0d;
            while (!found)
            {
                z = NextGaussian();
                u = NextDouble();
                v = Math.Pow(1 + c * z, 3d);
                bool firstCondition = z > -1d / c;
                bool secondCondition = Math.Log(u) < .5 * z * z + d - d * v + d * Math.Log(v);
                if (firstCondition)
                    if (secondCondition)
                        found = true;
            }
            return d * v;
        }

        private double GetRandomGammaForAnyShape(double shape)
        {
            if (shape >= 1)
                return GetRandomGammaForShapeGreaterThanOrEqualToOne(shape);
            else
            {
                double x = GetRandomGammaForShapeGreaterThanOrEqualToOne(shape + 1);
                return x * Math.Pow(NextDouble(), 1d / shape);
            }
        }


        /**
         * Returns a random deviate from a beta distribution.
         *  with scales scale1 and beta.
         * @param scale1 a double larger than 0
         * @param scale2 a double larger than 0
         * @return a double
         */
        public double NextBeta(double scale1, double scale2)
        {
            double x = NextGamma(scale1, 1d);
            double y = NextGamma(scale2, 1d);
            return x / (x + y);
        }


        /**
         * Returns a random gamma distributed value following Marsaglia and Tsang's method. The 
         * mean of the distribution is obtained through the product of the shape and the scale.
         * @param shape a double larger than 0
         * @param scale a double larger than 0
         * @return a double
         */
        public double NextGamma(double shape, double scale)
        {
            if (shape <= 0d || scale <= 0d)
            {
                throw new ArgumentException("The shape and the scale must be larger than 0!");
            }
            double x = GetRandomGammaForAnyShape(shape);
            return x * scale;
        }


        /**
         * This method returns a random integer that follows negative binomial distribution.
         * @param mean the mean of the distribution
         * @param dispersion the dispersion parameter
         * @return an integer
         */
        public int NextNegativeBinomial(double mean, double dispersion)
        {
            double threshold = NextDouble();    // to determine how many recruits there are
            double cumulativeProb = 0.0;
            int output = -1;

            while (threshold > cumulativeProb)
            {
                output++;
                double massProb = NegativeBinomialUtility.GetMassProbability(output, mean, dispersion);
                cumulativeProb += massProb;
            }
            return output;
        }


        /**
         * Returns a random deviate from the standard Student's t distribution. The algorithm behind 
         * the random deviate generation is that of Bailey (1994) based on polar generation.
         * @param df the degrees of freedom
         * @return a random deviate from the Student's t distribution
         * @see <a href="https://doi.org/10.1090/S0025-5718-1994-1219702-8"> Bailey, R.W. 1994. Polar generation of random variances with the t-distribution. 
         * Mathematics of Computation 62(206): 779-781.</a>
         */
        public double NextStudentT(double df)
        {
            double W = 2d;
            double U = 0;
            while (W > 1)
            {
                U = NextDouble();
                double V = NextDouble();
                U = 2 * U - 1;
                V = 2 * V - 1;
                W = U * U + V * V;
            }
            double C2 = U * U / W;
            double R2 = df * (Math.Pow(W, -2d / df) - 1);
            double result;
            if (NextDouble() < .5)
                result = Math.Sqrt(R2 * C2);
            else
                result = -Math.Sqrt(R2 * C2);
            return result;
        }


        /**
         * This method returns a Chi squared random value.
         * @param df the degrees of freedom
         * @return a double
         */
        public double NextChiSquare(int df)
        {
            if (df <= 0)
                throw new ArgumentException("The number of degrees of freedom should be larger than 0");
            return NextGamma(df * .5, 2d);
        }

        /**
         * This method returns the matrix A in the Bartlett decomposition.
         * @param df degrees of freedom
         * @param dim the dimensions of the matrix
         * @return a Matrix
         */
        public Matrix NextBartlettDecompositionMatrix(int df, int dim)
        {
            Matrix aMat = new(dim, dim);
            for (int i = 0; i < aMat.m_iRows; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                        aMat.SetValueAt(i, j, Math.Sqrt(NextChiSquare(df - i)));
                    else
                        aMat.SetValueAt(i, j, NextGaussian());
                }
            }
            return aMat;
        }

        ///**
        // * Return a random realization of a Weibull distribution. <br>
        // * <br>
        // * The method first draws a realization from a uniform distribution [0,1] and
        // * then computes the quantile.
        // * 
        // * @param k the shape parameter (must be greater than 0)
        // * @param lambda the scale parameter (must be greater than 0)
        // * @param theta the location parameter
        // * @return the realization
        // */
        //public double NextWeibull(double k, double lambda, double theta)
        //{
        //    return WeibullUtility.getQuantile(NextDouble(), k, lambda, theta);
        //}

        ///**
        // * Return a random realization of a Weibull distribution without location parameter. <br>
        // * <br>
        // * The method first draws a realization from a uniform distribution [0,1] and
        // * then computes the quantile.
        // * 
        // * @param k the shape parameter (must be greater than 0)
        // * @param lambda the scale parameter (must be greater than 0)
        // * @return the realization
        // */
        //public double NextWeibull(double k, double lambda)
        //{
        //    return NextWeibull(k, lambda, 0d);
        //}

        ///**
        // * Return a random realization of a truncated Gaussian distribution. <br>
        // * <br>
        // * The method first draws a realization from a uniform distribution [0,1] and
        // * then computes the quantile.
        // * 
        // * @param mu the mean of the original normal distribution
        // * @param sigma2 the variance of the original distribution
        // * @param a the lower bound
        // * @param b the upper bound
        // * @return the realization
        // */
        //public double NextTruncatedGaussian(double mu, double sigma2, double a, double b)
        //{
        //    return TruncatedGaussianUtility.GetQuantile(NextDouble(), mu, sigma2, a, b);
        //}
    }
}
