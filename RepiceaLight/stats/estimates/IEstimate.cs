using REpiceaLight.math;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.estimates
{
    public interface IEstimate : IMomentGettable, IDistributionProvider
    {


        /**
         * The type of estimator.
         * @author Mathieu Fortin - March 2012
s	 */
        public enum EstimatorType
        {
            Resampling,
            LeastSquares,
            LikelihoodBased,
            MomentBased,
            Unknown
        }

        /**
         * Provide the type of the estimator.
         * @return an EstimatorType instance
         */
        public EstimatorType GetEstimatorType();

        /**
         * Make it possible to set an optional row index.<p>
         * This is useful when the response is a vector. 
         * @param newRowIndex a List of String instance. A null value resets the row index.
         */
        public void SetRowIndex(List<String> newRowIndex);

        public Matrix GetRandomDeviate();

        /**
         * Return a copy of the row index. 
         * @return a List of String instance or null if the row index has not been set.
         */
        public List<string> GetRowIndex();



        /**
         * Return a difference estimate.
         * @param estimate2 an Estimate to be subtracted from this estimate.
         * @return an Estimate
         */
        public IEstimate GetDifferenceEstimate(IEstimate estimate2);

        /**
         * Return a sum of two estimates.
         * @param estimate2 an Estimate to be added to this estimate.
         * @return an Estimate
         */
        public IEstimate GetSumEstimate(IEstimate estimate2);


        /**
         * Return the product of this estimate by a scalar.
         * @param scalar a double to be multiplied by this estimate
         * @return an Estimate
         */
        public IEstimate GetProductEstimate(double scalar);

        /**
         * This method returns the probability of getting a lower valueand upper bound of a confidence intervals at probability
         * level 1 - alpha
         * @param oneMinusAlpha is 1 minus the probability of Type I error
         * @return a ConfidenceInterval instance 
         */
        public ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha);


        /**
         * Returns an estimate of the product of two parametric univariate estimate. The variance
         * estimator is based on Goodman's estimator.
         * @param estimate an Estimate instance
         * @return a SimpleEstimate instance
         */
        public IEstimate GetProductEstimate(IEstimate estimate);


        /**
         * Collapse the estimate following a map that contains the indices for each group. <p>
         * The collapsing ensures the consistency, that is all the row indices must be found in the
         * list instances contained in the map argument. If there is a mismatch, the method will throw an 
         * exception. IMPORTANT: the new indices, that is the keys of the map argument are sorted in the
         * new Estimate instance.
         * @param desiredIndicesForCollapsing a LinkedHashMap with the keys being the new indices and 
         * the values being lists of indices to be collapsed.
         * @return an Estimate instance
         */
        public IEstimate CollapseEstimate(OrderedDictionary desiredIndicesForCollapsing);



    }

}
