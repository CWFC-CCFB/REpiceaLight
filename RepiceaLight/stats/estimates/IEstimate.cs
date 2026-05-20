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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace REpiceaLight.stats.estimates
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

    public interface IEstimate : IMomentGettable, IDistributionProvider
    {


        /// <summary>
        /// Provide the type of the estimator.
        /// </summary>
        /// <returns>an EstimatorType instance</returns>        
        EstimatorType GetEstimatorType();

        /// <summary>
        /// Set an optional row index. This is useful when the response is a vector.
        /// </summary>
        /// <param name="newRowIndex">a List of String instance. A null value resets the row index</param>
        void SetRowIndex(List<String> newRowIndex);

        Matrix GetRandomDeviate();

        /// <summary>
        /// Provide a copy of the row index
        /// </summary>
        /// <returns>a List of String instance or null if the row index has not been set.</returns>
        List<string> GetRowIndex();

        /// <summary>
        /// Provide an estimate of the difference between this estimate.
        /// </summary>
        /// <param name="estimate2">an Estimate to be subtracted from this estimate</param>
        /// <returns>an IEstimate instance</returns>
        IEstimate GetDifferenceEstimate(IEstimate estimate2);

        /// <summary>
        /// Provide a sum of two estimates.
        /// </summary>
        /// <param name="estimate2">an Estimate to be added to this estimate.</param>
        /// <returns>An IEstimate instance</returns>
        IEstimate GetSumEstimate(IEstimate estimate2);

        /// <summary>
        /// Provide the product of this estimate by a scalar
        /// </summary>
        /// <param name="scalar">a double to be multiplied by this estimate</param>
        /// <returns>an IEstimate instance</returns>        
        IEstimate GetProductEstimate(double scalar);

         /// <summary>
        /// PRovide a confidence interval at probability level 1 - alpha.
        /// </summary>
        /// <param name="oneMinusAlpha">1 minus the probability of Type I error</param>
        /// <returns>a ConfidenceInterval instance</returns>
        ConfidenceInterval GetConfidenceIntervalBounds(double oneMinusAlpha);

        /// <summary>
        /// Provide an estimate of the product of two parametric univariate estimate. 
        /// The variance estimator is based on Goodman's estimator.
        /// </summary>
        /// <param name="estimate"> an IEstimate instance</param>
        /// <returns>an IEstimate instance</returns>
        IEstimate GetProductEstimate(IEstimate estimate);

        /// <summary>
        /// Collapse the estimate following a map that contains the indices for each group. The collapsing 
        /// ensures the consistency, that is all the row indices must be found in the list instances 
        /// contained in the map argument.If there is a mismatch, the method will throw an exception.
        /// IMPORTANT: the new indices, that is the keys of the map argument are sorted in the new 
        /// Estimate instance.
        /// </summary>
        /// <param name="desiredIndicesForCollapsing">a LinkedHashMap with the keys being the new indices and 
        /// the values being lists of indices to be collapsed.</param>
        /// <returns>an IEstimate instance</returns>
        IEstimate CollapseEstimate(OrderedDictionary desiredIndicesForCollapsing);



    }

}
