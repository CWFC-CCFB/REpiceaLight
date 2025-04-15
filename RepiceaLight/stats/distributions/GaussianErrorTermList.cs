using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public sealed class GaussianErrorTermList : List<GaussianErrorTerm>
    {


        internal bool updated;

        /**
         * This interface ensures the instance can return an index that will serve as distance for
         * the calculation of the variance-covariance matrix.
         * @author Mathieu Fortin - August 2014
         */
        public interface IIndexableErrorTerm
        {

            /**
             * This method returns the index of the error term. Typically, this value is the time and it serves to 
             * calculate the distance between two observations when computing the variance-covariance matrix.
             * @return an Integer
             */
            public int GetErrorTermIndex();
        }

        public List<int> GetDistanceIndex()
        {
            List<int> indexList = new();
            foreach (GaussianErrorTerm res in this)
            {
                indexList.Add(res.distanceIndex);
            }
            return indexList;
        }

        public Matrix GetNormalizedErrors()
        {
            Matrix mat = new(Count, 1);
            for (int i = 0; i < Count; i++)
                mat.SetValueAt(i, 0, this[i].normalizedValue);

            return mat;
        }

        internal Matrix GetRealizedErrors()
        {
            Matrix mat = new(Count, 1);
            for (int i = 0; i < Count; i++)
                mat.SetValueAt(i, 0, this[i].value);

            return mat;
        }

        public void UpdateErrorTerm(Matrix errorTerms)
        {
            for (int i = 0; i < errorTerms.m_iRows; i++)
            {
                GaussianErrorTerm error = this[i];
                if (error.value == null)
                {
                    error.value = errorTerms.GetValueAt(i, 0);
                }
            }
            updated = true;
        }

        public double GetErrorForIndexableInstance(IIndexableErrorTerm indexableErrorTerm)
        {
            int distanceIndex = indexableErrorTerm.GetErrorTermIndex();
            int index = GetDistanceIndex().IndexOf(distanceIndex);
            if (index < 0)
                throw new ArgumentException("This distance index is not contained in the GaussianErrorTermList");
            else
                return this[index].value;
        }


        public void Add(GaussianErrorTerm term)
        {
            base.Add(term);
            updated = false;
            Sort();
        }

    }


}
