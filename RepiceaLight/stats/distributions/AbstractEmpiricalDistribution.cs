using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.IDistribution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public abstract class AbstractEmpiricalDistribution : IDistribution
    {


        protected readonly List<Matrix> observations;

        /**
         * Constructor.
         */
        public AbstractEmpiricalDistribution()
        {
            observations = new();
        }

        /**
         * This method returns the number of observations in this nonparametric distribution.
         * @return an integer
         */
        public int GetNumberOfRealizations() { return observations.Count; }

        /**
         * This method sets a given observation of the nonparametric distribution.
         * @param value the value of the observation
         */
        public void AddRealization(Matrix value) { observations.Add(value); }

        /**
         * This method returns the array that contains all the observations of this distribution.
         * @return an array of Matrix instances
         */
        public List<Matrix> GetRealizations() { return observations; }

        public bool IsParametric()
        {
            return false;
        }

        public bool IsMultivariate()
        {
            if (observations != null && observations.Count > 0)
                return observations[0] is Matrix && observations[0].m_iRows > 1;
            else
                return false;
        }

        public DistributionType GetDistributionType() { 
            return DistributionType.NONPARAMETRIC; 
        }


        //	@Override
        //	public double getQuantile(double... values) {
        //		if (observationsgetM)
        //		// TODO to be implemented
        //		return -1;
        //	}

        public Matrix GetRandomRealization()
        {
            int observationIndex = (int)(StatisticalUtility.GetRandom().NextDouble() * GetNumberOfRealizations());
            return GetRealizations()[observationIndex];
        }

        public abstract Matrix GetMean();
        public abstract SymmetricMatrix GetVariance();
    }
}
