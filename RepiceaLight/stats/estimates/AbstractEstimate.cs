using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.estimates
{
    public abstract class AbstractEstimate : RandomVariable, IEstimate
    {


        //	private static final long serialVersionUID = 20120825L;

        protected IEstimate.EstimatorType estimatorType;

        protected readonly List<string> rowIndex;


        protected AbstractEstimate(IDistribution distribution) : base(distribution)
        {
            rowIndex = new();
        }


        public IEstimate.EstimatorType GetEstimatorType() { return estimatorType; }

        public void SetRowIndex(List<string> newRowIndex)
        {
            this.rowIndex.Clear();
            if (newRowIndex != null && newRowIndex.Count != 0)
            {
                if (newRowIndex.Count != GetMean().m_iRows)
                {
                    throw new ArgumentException("The size of the list is incompatible with tne dimension of the estimate!");
                }
                this.rowIndex.AddRange(newRowIndex);
            }
        }

        public List<string> GetRowIndex()
        {
            List<string> rowIndexCopy = new();
            rowIndexCopy.AddRange(rowIndex);
            return rowIndexCopy;
        }

        public Matrix GetRandomDeviate()
        {
            return GetDistribution().GetRandomRealization();
        }


        public virtual IEstimate GetDifferenceEstimate(IEstimate estimate2)
        {
            Matrix diff = GetMean().Subtract(estimate2.GetMean());
            SymmetricMatrix variance = SymmetricMatrix.ConvertToSymmetricIfPossible(GetVariance().Add(estimate2.GetVariance()));
            return new SimpleEstimate(diff, variance);
        }


        public virtual IEstimate GetSumEstimate(IEstimate estimate2)
        {
            Matrix diff = GetMean().Add(estimate2.GetMean());
            SymmetricMatrix variance = SymmetricMatrix.ConvertToSymmetricIfPossible(GetVariance().Add(estimate2.GetVariance()));
            return new SimpleEstimate(diff, variance);
        }


        public virtual IEstimate GetProductEstimate(double scalar)
        {
            Matrix diff = GetMean().ScalarMultiply(scalar);
            SymmetricMatrix variance = GetVariance().ScalarMultiply(scalar * scalar);
            return new SimpleEstimate(diff, variance);
        }

        public abstract ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha);

        /**
         * This method checks if the two point estimates are compatible. The basic
         * check consists of comparing the classes. Then, the matrix data is checked
         * for consistency with previous data.
         * @param estimate an Estimate instance
         * @return a boolean
         */
        protected virtual bool IsMergeableEstimate(IEstimate estimate)
        {
            return false;
        }


        public virtual IEstimate GetProductEstimate(IEstimate estimate)
        {
            if (estimate.GetDistribution().IsUnivariate() && GetDistribution().IsUnivariate())
            {
                Matrix alphaMean = GetMean();
                Matrix betaMean = estimate.GetMean();
                SymmetricMatrix alphaVariance = GetVariance();
                SymmetricMatrix betaVariance = estimate.GetVariance();
                Matrix newMean = alphaMean.Multiply(betaMean);
                Matrix newVariance = alphaMean.ElementWisePower(2d).Multiply(betaVariance).
                        Add(betaMean.ElementWisePower(2d).Multiply(alphaVariance)).
                        Subtract(alphaVariance.Multiply(betaVariance));
                return new SimpleEstimate(newMean, SymmetricMatrix.ConvertToSymmetricIfPossible(newVariance));
            }
            throw new InvalidOperationException("The getProductEstimate is only implemented for parametric univariate distribution ");
        }

        //  /**
        //* A static method to compute the product of many estimates.
        //* @param estimates a list of Estimate instances
        //* @return a SimpleEstimate instance
        //*/
        //  public static SimpleEstimate getProductOfManyEstimates(List<Estimate<Matrix, SymmetricMatrix, ?>> estimates)
        //  {
        //      Estimate < Matrix, SymmetricMatrix, ?> currentEstimate = null;
        //      for (int i = 1; i < estimates.size(); i++)
        //      {
        //          if (i == 1)
        //          {
        //              currentEstimate = estimates.get(i - 1);
        //          }
        //          currentEstimate = currentEstimate.getProductEstimate(estimates.get(i));
        //      }
        //      return (SimpleEstimate)currentEstimate;
        //  }

        public IEstimate CollapseEstimate(OrderedDictionary desiredIndicesForCollapsing)
        {
            return CollapseMeanAndVariance(desiredIndicesForCollapsing);
        }

        SimpleEstimate CollapseMeanAndVariance(OrderedDictionary desiredIndicesForCollapsing) {
            Matrix mean = GetMean();
            if (rowIndex.Count == 0)
                throw new InvalidOperationException("The row indices have not been set yet!");
            if (rowIndex.Count != mean.m_iRows)
                throw new ArgumentException("The size of the list is incompatible with the dimension of the estimate!");
            List<string> copyOfIndex = new();
            copyOfIndex.AddRange(GetRowIndex());
            copyOfIndex.Sort();
            List<string> completeList = new();
            foreach (List<string> l in desiredIndicesForCollapsing.Values)
                completeList.AddRange(l);
            completeList.Sort();
            if (!completeList.SequenceEqual<string>(copyOfIndex))
                throw new ArgumentException("Some indices are missing in the desiredIndicesForCollapsing or cannot be found in the row indices!");

            Matrix newMean = CollapseRowVector(mean, desiredIndicesForCollapsing);

            SymmetricMatrix oldVariance = GetVariance();
            SymmetricMatrix newVariance = CollapseSquareMatrix(oldVariance, desiredIndicesForCollapsing);

            SimpleEstimate outputEstimate = new SimpleEstimate(newMean, newVariance);

            List<string> newIndexRow = GetKeysFromOrderedDict(desiredIndicesForCollapsing);
            newIndexRow.Sort();
            outputEstimate.SetRowIndex(newIndexRow);

            return outputEstimate;
        }

        private static List<string> GetKeysFromOrderedDict(OrderedDictionary dict)
        {
            List<string> outputList = new();
            foreach (object k in dict.Keys)
            {
                outputList.Add((string) k);
            }
            return outputList;
        }

        Matrix CollapseRowVector(Matrix originalMatrix, OrderedDictionary desiredIndicesForCollapsing)
        {
            List<string> newIndexRow = GetKeysFromOrderedDict(desiredIndicesForCollapsing);
            newIndexRow.Sort();
            Matrix collapsedMatrix = new(desiredIndicesForCollapsing.Count, 1);
            for (int i = 0; i < collapsedMatrix.m_iRows; i++)
            {
                List<string> requestedIndices = (List<string>) desiredIndicesForCollapsing[newIndexRow[i]];
                collapsedMatrix.SetValueAt(i, 0, originalMatrix.GetSubMatrix(ConvertIndexIntoInteger(requestedIndices), null).GetSumOfElements());
            }
            return collapsedMatrix;
        }

        SymmetricMatrix CollapseSquareMatrix(Matrix originalMatrix, OrderedDictionary desiredIndicesForCollapsing)
        {
            if (originalMatrix == null) // means the variance cannot be calculated as in the MonteCarloEstimate class if the nb realizations is smaller than 2.
                return null;

            List<string> newIndexRow = GetKeysFromOrderedDict(desiredIndicesForCollapsing);
            newIndexRow.Sort();
            Matrix collapsedMatrix = new(desiredIndicesForCollapsing.Count, desiredIndicesForCollapsing.Count);
            for (int i = 0; i < collapsedMatrix.m_iRows; i++)
            {
                List<string> requestedIndices_i = (List<string>)desiredIndicesForCollapsing[newIndexRow[i]];
                for (int j = 0; j < collapsedMatrix.m_iRows; j++)
                {
                    List<string> requestedIndices_j = (List<string>)desiredIndicesForCollapsing[newIndexRow[j]];
                    Matrix tmpMatrix = originalMatrix.GetSubMatrix(ConvertIndexIntoInteger(requestedIndices_i),
                            ConvertIndexIntoInteger(requestedIndices_j));
                    collapsedMatrix.SetValueAt(i, j, tmpMatrix.GetSumOfElements());
                }
            }
            return SymmetricMatrix.ConvertToSymmetricIfPossible(collapsedMatrix);
        }


        private List<int> ConvertIndexIntoInteger(List<string> selectedIndices)
        {
            List<int> outputList = new();
            foreach (string s in selectedIndices)
                outputList.Add(GetRowIndex().IndexOf(s));
            return outputList;
        }

        public sealed override Matrix GetMean()
        {
            return base.GetMean();
        }

        public sealed override SymmetricMatrix GetVariance()
        {
            return base.GetVariance();
        }

    }

}
