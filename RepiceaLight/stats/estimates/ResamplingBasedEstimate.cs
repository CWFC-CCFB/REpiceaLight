using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.estimates
{

    public abstract class ResamplingBasedEstimate : AbstractEstimate, INumberOfRealizationsProvider
    {

        /**
         * Constructor.
         */
        protected ResamplingBasedEstimate(AbstractEmpiricalDistribution dist) : base(dist)
        {
            estimatorType = IEstimate.EstimatorType.Resampling;
        }


        /**
         * Add a realization to the empirical distribution.<p>
         * The method checks the conformity of the value argument,
         * which must be a column vector to ensure a proper variance
         * estimation. 
         * @param value a Matrix instance
         */
        public void AddRealization(Matrix value)
        {
            if (CheckConformity(value))
                GetDistribution().AddRealization(value);
            else
                throw new InvalidOperationException("The matrix is not conform to previous observations!");
        }

        public override AbstractEmpiricalDistribution GetDistribution()
        {
            return (AbstractEmpiricalDistribution) base.GetDistribution();
        }

        private bool CheckConformity(Matrix value)
        {
            if (value == null)
                throw new ArgumentException("The value argument must be a non null Matrix instance!");
            List<Matrix> observations = GetDistribution().GetRealizations();
            if (observations.Count == 0)
                return true;
            else
            {
                Matrix firstObservation = observations[0];
                return firstObservation.m_iRows == value.m_iRows && firstObservation.m_iCols == value.m_iCols;
            }
        }


        /**
         * Provide the quantile associated to a particular probability. 
         * @param probability the probability level
         * @return a Matrix instance that contains the quantiles
         */
        internal abstract Matrix GetQuantileForProbability(double probability);

        public override ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha)
        {
            Matrix lowerBoundValue = GetQuantileForProbability(.5 * (1d - oneMinusAlpha));
            Matrix upperBoundValue = GetQuantileForProbability(1d - .5 * (1d - oneMinusAlpha));
            return new ConfidenceInterval(lowerBoundValue, upperBoundValue, oneMinusAlpha);
        }

        /**
         * This method returns the list of realizations in the empirical distribution.
         * @return a List of Matrix instance
         */
        public List<Matrix> GetRealizations() { return GetDistribution().GetRealizations(); }

        public int GetNumberOfRealizations() { return GetDistribution().GetNumberOfRealizations(); }

    }
}
