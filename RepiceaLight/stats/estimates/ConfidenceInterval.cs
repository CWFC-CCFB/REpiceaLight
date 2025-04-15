using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.estimates
{

    public class ConfidenceInterval
    {


        public class CIBound : BasicBound
        {

            internal CIBound(bool isUpperBound) : base(isUpperBound)
            {
            }

            internal override void SetBoundValue(Matrix value)
            {
                base.SetBoundValue(value);
            }

            public override Matrix GetBoundValue()
            {
                return base.GetBoundValue();
            }
        }

        private readonly CIBound lowerBound;
        private readonly CIBound upperBound;
        private readonly double probabilityLevel;

        /**
         * Constructor.
         * @param lowerBoundValue a Matrix instance standing for the lower bound
         * @param upperBoundValue a Matrix instance standing for the upper bound
         * @param probabilityLevel the probability level associated with these bounds (e.g. 0.95)
         */
        public ConfidenceInterval(Matrix lowerBoundValue, Matrix upperBoundValue, double probabilityLevel)
        {
            lowerBound = new CIBound(false);
            upperBound = new CIBound(true);
            lowerBound.SetBoundValue(lowerBoundValue);
            upperBound.SetBoundValue(upperBoundValue);
            this.probabilityLevel = probabilityLevel;
        }

        /**
         * This method returns the lower bound of the interval.
         * @return a Matrix instance
         */
        public Matrix GetLowerLimit() { return lowerBound.GetBoundValue(); }

        /**
         * This method returns the upper bound of the interval.
         * @return a Matrix instance
         */
        public Matrix GetUpperLimit() { return upperBound.GetBoundValue(); }

        /**
         * This method returns the probability level of the interval.
         * @return a double
         */
        public double GetProbabilityLevel() { return probabilityLevel; }


        /**
         * This method returns true if one of the bound of the confidence intervals contains a NaN
         * @return a boolean
         */
        public bool IsThereAnyNaN()
        {
            return GetLowerLimit().DoesContainAnyNaN() || GetUpperLimit().DoesContainAnyNaN();
        }

    }

}
