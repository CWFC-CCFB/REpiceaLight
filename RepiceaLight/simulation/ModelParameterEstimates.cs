using REpiceaLight.math;
using REpiceaLight.stats;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.simulation
{
    public class ModelParameterEstimates : GaussianEstimate
    {


        protected readonly List<int> estimatedParameterIndices;

        /**
         * Constructor.
         * @param mean a vector that corresponds to the mean value
         * @param variance a symmetric positive definite matrix 
         */
        public ModelParameterEstimates(Matrix mean, SymmetricMatrix variance) : base(mean, variance)
        {
            estimatedParameterIndices = new();
            SetEstimatedParameterIndices();
        }

        protected virtual void SetEstimatedParameterIndices()
        {
            for (int i = 0; i < GetMean().m_iRows; i++)
                estimatedParameterIndices.Add(i);
        }

        /**
         * This method returns the indices of the parameters that were truly estimated. In the case of 
         * a SAS implementation, some parameters may actually be fake.. 
         * @return a List of Integer which is a copy of the original list to avoid modifications.
         */
        public List<int> GetTrueParameterIndices()
        {
            List<int> copyList = new();
            copyList.AddRange(estimatedParameterIndices);
            return copyList;
        }

        public new Matrix GetRandomDeviate()
        {
            Matrix lowerChol = GetDistribution().GetStandardDeviation();
            Matrix randomVector = StatisticalUtility.DrawRandomVector(lowerChol.m_iRows, IDistribution.DistributionType.GAUSSIAN);
            Matrix oMat = lowerChol.Multiply(randomVector);
            Matrix deviate = GetMean().Clone();
            deviate.AddElementsAt(estimatedParameterIndices, oMat);
            return deviate;
        }

    }
}
