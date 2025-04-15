using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats
{
    public interface IMomentGettable
    {

        /**
         * This method returns the first central moment, i.e. the mean vector, of the random variable.
         * @return an AbstractMatrix-derived instance
         */
        public Matrix GetMean();

        /**
         * This method returns the second central moment, i.e. the variance-covariance matrix, of the random variable.
         * @return an AbstractMatrix-derived instance
         */
        public SymmetricMatrix GetVariance();


    }

}
