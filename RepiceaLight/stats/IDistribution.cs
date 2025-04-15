using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats
{
    public interface IDistribution : IMomentGettable
    {

        public enum DistributionType
        {
            GAUSSIAN,
            UNIFORM,
            NONPARAMETRIC,
            UNKNOWN,
            CHI_SQUARE,
            WISHART,
            STUDENT
        }

        /**
         * This method returns true if the distribution is parametric or false otherwise.
         * @return a boolean
         */
        public abstract bool IsParametric();


        /**
         * This method returns true if the GaussianFunction instance is multivariate.
         * @return a boolean
         */
        public abstract bool IsMultivariate();


        /**
         * Returns true if the GaussianFunction instance is univariate.
         * @return a boolean
         */
        public virtual bool IsUnivariate()
        {
            return !IsMultivariate();
        }

        /** 
         * This method returns the type of the distribution.
         * @return a Type enum
         */
        public abstract DistributionType GetDistributionType();


        /**
         * This method draws a random realization from the distribution.
         * @return the observation in a Matrix instance
         */
        public abstract Matrix GetRandomRealization();


    }
}
