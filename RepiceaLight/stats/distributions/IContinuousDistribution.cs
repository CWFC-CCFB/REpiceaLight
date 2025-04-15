using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.distributions
{
    public interface IContinuousDistribution : IDistribution
    {

        /**
         * Return the probability density of the values parameter.
         * @param values a Matrix consistent with the distribution parameters
         * @return a double
         */
        public abstract double GetProbabilityDensity(Matrix values);

    }
}
