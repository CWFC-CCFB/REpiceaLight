using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.estimates
{
    public interface IDistributionProvider 
    {

        /**
         * Provide the assumed distribution for the random variable.
         * @return a Distribution-derived instance
         */
        public IDistribution GetDistribution();

    }
}
