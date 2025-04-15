using REpiceaLight.math.utility;
using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.estimates.IEstimate;
using REpiceaLight.stats.distributions;

namespace REpiceaLight.stats.estimates
{
    public class GaussianEstimate : AbstractEstimate, IMomentSettable
    {

        /**
         * Common constructor. By default the Gaussian distribution that supports this estimate has a mean 0 and a variance 1.
         */
        public GaussianEstimate() : base(new GaussianDistribution())
        {
            estimatorType = EstimatorType.LikelihoodBased;
        }

        /**
         * Constructor with the mean and variance.
         * @param mean a Matrix instance that contains the mean 
         * @param variance a SymmetricMatrix instance that contains the variance-covariance
         */
        public GaussianEstimate(Matrix mean, SymmetricMatrix variance) : this()
        {
            SetMean(mean);
            SetVariance(variance);
        }

        /**
         * Constructor for univariate distribution.
         * @param mean a double that stands for the mean
         * @param variance a double that stands for the variance
         */
        public GaussianEstimate(double mean, double variance) : this()
        {
            Matrix meanMat = new(1, 1);
            meanMat.SetValueAt(0, 0, mean);
            SymmetricMatrix varianceMat = new SymmetricMatrix(1);
            varianceMat.SetValueAt(0, 0, variance);
            SetMean(meanMat);
            SetVariance(varianceMat);
        }


        public new GaussianDistribution GetDistribution()
        {
            return (GaussianDistribution)base.GetDistribution();
        }

        public void SetVariance(SymmetricMatrix variance)
        {
            GetDistribution().SetVariance(variance);
        }

        public void SetMean(Matrix mean)
        {
            GetDistribution().SetMean(mean);
        }

        protected Matrix GetQuantileForProbability(double probability)
        {
            Matrix stdDev = GetVariance().DiagonalVector().ElementWisePower(.5);
            double quantile = GaussianUtility.GetQuantile(probability);
            return GetMean().Add(stdDev.ScalarMultiply(quantile));
        }

        public override ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha)
        {
            Matrix lowerBoundValue = GetQuantileForProbability(.5 * (1d - oneMinusAlpha));
            Matrix upperBoundValue = GetQuantileForProbability(1d - .5 * (1d - oneMinusAlpha));
            return new ConfidenceInterval(lowerBoundValue, upperBoundValue, oneMinusAlpha);
        }

    }

}
