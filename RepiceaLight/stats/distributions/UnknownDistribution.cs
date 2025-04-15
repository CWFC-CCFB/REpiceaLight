using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.IDistribution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public class UnknownDistribution : IDistribution, IMomentSettable
    {


        private Matrix mean;
        private SymmetricMatrix variance;


        public bool IsMultivariate()
        {
            return mean.m_iRows > 1;
        }

        public Matrix GetMean()
        {
            return mean;
        }

        public SymmetricMatrix GetVariance()
        {
            return variance;
        }

        public DistributionType GetDistributionType()
        {
            return DistributionType.UNKNOWN;
        }

        public void SetMean(Matrix mean)
        {
            this.mean = mean;
        }

        public void SetVariance(SymmetricMatrix variance)
        {
            this.variance = variance;
        }

        public bool IsParametric()
        {
            return false;
        }

        public Matrix GetRandomRealization()
        {
            return null;
        }

        //	@Override
        //	public double getQuantile(double... values) {
        //		return -1;
        //	}


    }

}
