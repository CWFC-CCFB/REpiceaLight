using REpiceaLight.math;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats
{
    public abstract class RandomVariable : IMomentGettable, IDistributionProvider
    {



        private readonly IDistribution distribution;

        protected RandomVariable(IDistribution distribution)
        {
            this.distribution = distribution;
        }

        public virtual IDistribution GetDistribution()
        {
            return distribution;
        }

        public virtual Matrix GetMean()
        {
            return GetMeanFromDistribution();
        }

        protected Matrix GetMeanFromDistribution()
        {
            return GetDistribution().GetMean();
        }

        public virtual SymmetricMatrix GetVariance()
        {
            return GetVarianceFromDistribution();
        }

        protected SymmetricMatrix GetVarianceFromDistribution()
        {
            return GetDistribution().GetVariance();
        }

    }

}
