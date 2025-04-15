using REpiceaLight.math.utility;
using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.estimates.IEstimate;
using static REpiceaLight.stats.StatisticalUtility;
using REpiceaLight.stats.distributions;

namespace REpiceaLight.stats.estimates
{
    public class GaussianErrorTermEstimate : AbstractEstimate
    {

        /**
         * General constructor.
         * @param variance a double
         * @param correlationParameter a double 
         * @param type a TypeMatrixR enum
         */
        public GaussianErrorTermEstimate(SymmetricMatrix variance, double correlationParameter, TypeMatrixR type) : base(new CenteredGaussianDistribution(variance, correlationParameter, type))
        {
            estimatorType = EstimatorType.LikelihoodBased;
        }

        /**
         * Constructor for univariate distribution.
         * @param variance a double
         */
        public GaussianErrorTermEstimate(SymmetricMatrix variance) : base(new CenteredGaussianDistribution(variance))
        {
        }

        public override CenteredGaussianDistribution GetDistribution()
        {
            return (CenteredGaussianDistribution)base.GetDistribution();
        }


        public Matrix GetMean(GaussianErrorTermList errorTermList)
        {
            return GetDistribution().GetMean(errorTermList);
        }

        /**
         * Provide the variance of the distribution given some error terms.<p>
         * The class adapts the variance matrix as the number of error terms increases.
         * @param errorTermList a GaussianErrorTermList instance
         * @return a SymmetricMatrix instance
         */
        public SymmetricMatrix GetVariance(GaussianErrorTermList errorTermList)
        {
            return GetDistribution().GetVariance(errorTermList);
        }

        public Matrix GetRandomDeviate(GaussianErrorTermList errorTermList)
        {
            return GetDistribution().GetRandomRealization(errorTermList);
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
