using REpiceaLight.math;

namespace REpiceaLight.simulation
{
    public class SASParameterEstimates : ModelParameterEstimates
    {


        /**
         * Constructor.
         * @param mean a vector that corresponds to the mean value
         * @param variance a symmetric positive definite matrix 
         */
        public SASParameterEstimates(Matrix mean, SymmetricMatrix variance) : base(mean, variance)
        {
        }

        protected override void SetEstimatedParameterIndices()
        {
            Matrix mean = GetMean();
            for (int i = 0; i < mean.m_iRows; i++)
            {
                if (mean.GetValueAt(i, 0) != 0d && mean.GetValueAt(i, 0) != 1d)
                {
                    estimatedParameterIndices.Add(i);
                }
            }
            Matrix variance = GetVariance();
            if (variance != null && variance.m_iRows != estimatedParameterIndices.Count)
            {
                throw new ArgumentException("SASParameterEstimates: the variance matrix is not compatible with the vector of parameter estimates");
            }
        }

    }



}

