/*
 * This file is part of the REpiceaLight library.
 *
 * Copyright (C) 2026 His Majesty the King in right of Canada
 * Author: Mathieu Fortin, Canadian Forest Service
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed with the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE. See the GNU Lesser General Public
 * License for more details.
 *
 * Please see the license at http://www.gnu.org/copyleft/lesser.html.
 */
using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using System;
using System.Collections.Generic;

namespace REpiceaLight.stats.estimates
{
    public class MonteCarloEstimate : ResamplingBasedEstimate {

        /// <summary>
        /// Constructor.
        /// </summary>
        public MonteCarloEstimate() : base(new EmpiricalDistribution()) { }

        /// <summary>
        /// Provide a MonteCarloEstimate instance that results from the subtraction of two 
        /// MonteCarloEstimate instances with the same number of realizations.
        /// </summary>
        /// <param name="estimate2">the estimate that is subtracted from this estimate</param>
        /// <returns>a MonteCarloEstimate instance</returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected MonteCarloEstimate Subtract(MonteCarloEstimate estimate2)
        {
            if (GetNumberOfRealizations() != estimate2.GetNumberOfRealizations())
                throw new InvalidOperationException("The number of realizations is not consistent!");
            MonteCarloEstimate outputEstimate = new MonteCarloEstimate();
            for (int i = 0; i < GetNumberOfRealizations(); i++)
                outputEstimate.AddRealization(GetRealizations()[i].Subtract(estimate2.GetRealizations()[i]));
            return outputEstimate;
        }

 
        /// <summary>
        /// Provide a MonteCarloEstimate instance that results from the sum of two
        /// MonteCarloEstimate instances with the same number of realizations.
        /// </summary>
        /// <param name="estimate2">the estimate that is added to this estimate</param>
        /// <returns>a MonteCarloEstimate instance</returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected MonteCarloEstimate Add(MonteCarloEstimate estimate2)
        {
            if (GetNumberOfRealizations() != estimate2.GetNumberOfRealizations())
                throw new InvalidOperationException("The number of realizations is not consistent!");
            MonteCarloEstimate outputEstimate = new MonteCarloEstimate();
            for (int i = 0; i < GetNumberOfRealizations(); i++)
                outputEstimate.AddRealization(GetRealizations()[i].Add(estimate2.GetRealizations()[i]));
            return outputEstimate;
        }


        /// <summary>
        /// Provide a MonteCarloEstimate instance that results from the product of original 
        /// MonteCarloEstimate instance and a scalar.
        /// </summary>
        /// <param name="scalar">the multiplication factor</param>
        /// <returns>a MonteCarloEstimate instance</returns>
        protected MonteCarloEstimate Multiply(double scalar)
        {
            MonteCarloEstimate outputEstimate = new MonteCarloEstimate();
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
            Matrix percentileValues = new Matrix(nbRows, 1);
            for (int i = 0; i < nbRows; i++)
            {
                realizationsForThisRow = new List<double>();
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

        /// <summary>
        /// This method returns a subset of the MonteCarloEstimate. For instance, if the estimate 
        /// is multivariate, it is then possible to extract a MonteCarloEstimate with only the first 
        /// and second variates.
        /// </summary>
        /// <param name="indices">a List of Integer that are the indices of the variates to be extracted.</param>
        /// <returns>a MonteCarloEstimate instance</returns>
        public MonteCarloEstimate ExtractSubEstimate(List<int> indices)
        {
            MonteCarloEstimate subEstimate = new MonteCarloEstimate();
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

        public override string ToString()
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
