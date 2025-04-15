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
    public class MonteCarloEstimate : ResamplingBasedEstimate {


//    public enum MessageID implements TextableEnum
//    {
//        Mean("Mean", "Moyenne"),
//		Lower("Lower", "Inf"),
//		Upper("Upper", "Sup"),
//		ProbabilityLevel("Probability level", "Niveau de probabilit\u00E9");

//        MessageID(String englishText, String frenchText) {
//            setText(englishText, frenchText);
//        }

//        @Override

//        public void setText(String englishText, String frenchText)
//    {
//        REpiceaTranslator.setString(this, englishText, frenchText);
//    }

//    @Override
//        public String toString() { return REpiceaTranslator.getString(this); }

//}

        /**
        * Constructor.
        */
        public MonteCarloEstimate() : base(new EmpiricalDistribution())
        {

        }

        /**
         * This method returns a MonteCarloEstimate instance that results from the subtraction of two 
         * MonteCarloEstimate instances with the same number of realizations. 
         * @param estimate2 the estimate that is subtracted from this estimate
         * @return a MonteCarloEstimate instance
         */
        protected MonteCarloEstimate Subtract(MonteCarloEstimate estimate2)
        {
            if (GetNumberOfRealizations() != estimate2.GetNumberOfRealizations())
                throw new InvalidOperationException("The number of realizations is not consistent!");
            MonteCarloEstimate outputEstimate = new();
            for (int i = 0; i < GetNumberOfRealizations(); i++)
                outputEstimate.AddRealization(GetRealizations()[i].Subtract(estimate2.GetRealizations()[i]));
            return outputEstimate;
        }

        /**
         * This method returns a MonteCarloEstimate instance that results from the sum of two 
         * MonteCarloEstimate instances with the same number of realizations. 
         * @param estimate2 the estimate that is added to this estimate
         * @return a MonteCarloEstimate instance
         */
        protected MonteCarloEstimate Add(MonteCarloEstimate estimate2)
        {
            if (GetNumberOfRealizations() != estimate2.GetNumberOfRealizations())
                throw new InvalidOperationException("The number of realizations is not consistent!");
            MonteCarloEstimate outputEstimate = new();
            for (int i = 0; i < GetNumberOfRealizations(); i++)
                outputEstimate.AddRealization(GetRealizations()[i].Add(estimate2.GetRealizations()[i]));
            return outputEstimate;
        }

        /**
         * This method returns a MonteCarloEstimate instance that results from the product of original 
         * MonteCarloEstimate instance and a scalar. 
         * @param scalar the multiplication factor
         * @return a MonteCarloEstimate instance
         */
        protected MonteCarloEstimate Multiply(double scalar)
        {
            MonteCarloEstimate outputEstimate = new();
            for (int i = 0; i < GetNumberOfRealizations(); i++)
                outputEstimate.AddRealization(GetRealizations()[i].ScalarMultiply(scalar));
            return outputEstimate;
        }


        internal override Matrix GetQuantileForProbability(double probability)
        {
            if (probability < 0 || probability > 1)
                throw new ArgumentException("The percentile must be between 0 and 1!");
            List<Matrix> realizations = GetRealizations();
            List<double> realizationsForThisRow;
            int nbRows = realizations[0].m_iRows;
            Matrix percentileValues = new(nbRows, 1);
            for (int i = 0; i < nbRows; i++)
            {
                realizationsForThisRow = new();
                for (int j = 0; j < realizations.Count; j++)
                {
                    realizationsForThisRow.Add(realizations[j].GetValueAt(i, 0));
                }
                realizationsForThisRow.Sort();
                int index = (int)Math.Round(probability * realizations.Count) - 1;
                if (index < 0)
                    index = 0;
                percentileValues.SetValueAt(i, 0, realizationsForThisRow[index]);
            }
            return percentileValues;
        }


        /**
         * This method returns a subset of the MonteCarloEstimate. For instance, if the estimate is multivariate, it is then possible
         * to extract a MonteCarloEstimate with only the first and second variates. 
         * @param indices a List of Integer that are the indices of the variates to be extracted.
         * @return a MonteCarloEstimate
         */
        public MonteCarloEstimate ExtractSubEstimate(List<int> indices)
        {
            MonteCarloEstimate subEstimate = new();
            foreach (Matrix realization in GetRealizations())
            {
                if (realization.IsColumnVector())
                    subEstimate.AddRealization(realization.GetSubMatrix(indices, null));
                else
                    subEstimate.AddRealization(realization.GetSubMatrix(null, indices));
            }
            return subEstimate;
        }

        protected override bool IsMergeableEstimate(IEstimate estimate)
        {
            if (estimate is MonteCarloEstimate) {
                if (((MonteCarloEstimate)estimate).GetNumberOfRealizations() == GetNumberOfRealizations())
                    return true;
            }
            return false;
        }

        public string ToString()
        {
            return "Monte Carlo estimate (mean = " + GetMean() + ", n = " + GetNumberOfRealizations();
        }



        public override IEstimate GetDifferenceEstimate(IEstimate estimate2)
        {
            if (IsMergeableEstimate(estimate2))
                return Subtract((MonteCarloEstimate) estimate2);
            else
                return base.GetDifferenceEstimate(estimate2);
        }

        public override IEstimate GetSumEstimate(IEstimate estimate2)
        {
            if (IsMergeableEstimate(estimate2))
                return Add((MonteCarloEstimate) estimate2);
            else
                return base.GetSumEstimate(estimate2);
        }

        public override IEstimate GetProductEstimate(double scalar)
        {
            return Multiply(scalar);
        }



    }

}
