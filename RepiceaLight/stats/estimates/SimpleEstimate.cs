using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.estimates
{
    public class SimpleEstimate : AbstractEstimate, IMomentSettable
    {


        /**
         * Public constructor 1 for derived classes.
         */
        public SimpleEstimate() : base(new UnknownDistribution())
        {
            estimatorType = IEstimate.EstimatorType.Unknown;
        }


        public override UnknownDistribution GetDistribution()
        {
            return (UnknownDistribution) base.GetDistribution();
        }

        /**
         * Public constructor 2 with mean and variance
         * @param mean a Matrix instance
         * @param variance a Matrix instance
         */
        public SimpleEstimate(Matrix mean, SymmetricMatrix variance) : this()
        {
            SetMean(mean);
            SetVariance(variance);
        }

        public void SetMean(Matrix mean)
        {
            GetDistribution().SetMean(mean);
        }

        public void SetVariance(SymmetricMatrix variance)
        {
            GetDistribution().SetVariance(variance);
        }


        public override ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha)
        {
            return null; // as no specific distribution is assumed
        }


    }

}
