using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.distributions.GaussianErrorTermList;

namespace REpiceaLight.stats.distributions
{
    public class GaussianErrorTerm : IComparable
    {

        internal readonly int distanceIndex;
        internal double value;
        internal readonly double normalizedValue;

        public GaussianErrorTerm(IIndexableErrorTerm caller) : this(caller, StatisticalUtility.GetRandom().NextGaussian())
        {
        }

        public GaussianErrorTerm(IIndexableErrorTerm caller, double normalizedValue)
        {
            this.distanceIndex = ((IIndexableErrorTerm)caller).GetErrorTermIndex();
            this.normalizedValue = normalizedValue;
        }


        public int CompareTo(object? obj)
        {
            if (obj == null || obj is not GaussianErrorTerm)
                throw new ArgumentException("The object parameter should be an instance of GaussianErrorTerm class");

            GaussianErrorTerm errorTerm = (GaussianErrorTerm)obj;
            if (this.distanceIndex < errorTerm.distanceIndex)
                return -1;
            else if (this.distanceIndex == errorTerm.distanceIndex)
                return 0;
            else
                return 1;
        }


    }
}
