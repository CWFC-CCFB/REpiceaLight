using REpiceaLight.math.utility;
using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public class StandardGaussianDistribution : IContinuousDistribution
    {

        private static StandardGaussianDistribution Singleton;

        private Matrix mu;
        private SymmetricMatrix sigma2;
        private Matrix lowerCholTriangle;

        /**
         * This constructor creates a Gaussian distribution with mean mu 0 and variance 1.
         */
        internal StandardGaussianDistribution()
        {
            Matrix mu = new(1, 1);
            SetMean(mu);
            SymmetricMatrix sigma2 = new(1);
            sigma2.SetValueAt(0, 0, 1d);
            SetVariance(sigma2);
        }

        protected virtual void SetMean(Matrix mu)
        {
            this.mu = mu;   
        }

        protected void SetVariance(SymmetricMatrix sigma2)
        {
            this.sigma2 = sigma2;
        }

        /**
         * This method returns the single instance of the StandardGaussianDistribution class.
         * @return a StandardGaussianDistribution instance
         */
        public static StandardGaussianDistribution GetInstance()
        {
            Singleton ??= new StandardGaussianDistribution();
            return Singleton;
        }

        public bool IsMultivariate()
        {
            return GetMu().m_iRows > 1;
        }

        public Matrix GetRandomRealization()
        {
            Matrix mean = GetMean();
            Matrix standardDeviation = GetStandardDeviation();
            Matrix normalStandardDeviates = StatisticalUtility.DrawRandomVector(standardDeviation.m_iRows,
                IDistribution.DistributionType.GAUSSIAN);
            return mean.Add(standardDeviation.Multiply(normalStandardDeviates));
        }

        /**
         * This method returns the lower triangle of the Cholesky decomposition of the variance-covariance matrix.
         * @return a Matrix instance
         */
        public Matrix GetStandardDeviation()
        {
            if (lowerCholTriangle == null)
            {
                lowerCholTriangle = GetSigma2().GetLowerCholTriangle();
            }
            return lowerCholTriangle;
        }

        public Matrix GetMean() { return GetMu(); }

        public SymmetricMatrix GetVariance() { return GetSigma2(); }

        public IDistribution.DistributionType GetDistributionType() { return IDistribution.DistributionType.GAUSSIAN; }

        protected Matrix GetMu() { return mu; }

        protected SymmetricMatrix GetSigma2() { return (SymmetricMatrix)sigma2; }

        public bool IsParametric() { return true; }


        /**
         * This method returns the result of the probability density function of the distribution parameter.
         * @param yValues a single double value or a Matrix instance
         * @return a double
         */
        public double GetProbabilityDensity(Matrix yValues)
        {
            if (yValues == null || !yValues.IsTheSameDimension(GetMu()))
                throw new InvalidOperationException("Vector y is either null or its dimensions are different from those of mu!");
            else
            {
                if (!IsMultivariate())
                {
                    double y = yValues.GetValueAt(0, 0);
                    double mu = GetMu().GetValueAt(0, 0);
                    double variance = GetSigma2().GetValueAt(0, 0);
                    return GaussianUtility.GetProbabilityDensity(y, mu, variance);
                }
                else
                {
                    int k = yValues.m_iRows;
                    Matrix residuals = yValues.Subtract(GetMu());
                    Matrix invSigma2 = GetSigma2().GetInverseMatrix();
                    return 1d / (Math.Pow(2 * Math.PI, 0.5 * k) * Math.Sqrt(GetSigma2().GetDeterminant())) * Math.Exp(-0.5 * residuals.Transpose().Multiply(invSigma2).Multiply(residuals).GetSumOfElements());
                }
            }
        }


    }

}
