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
using REpiceaLight.stats.estimates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace REpiceaLight.simulation.hdrelationships
{


    public class RegressionElements
    {
        public Matrix vectorZ;
        public double fixedPred;
        public Enum species;

        public RegressionElements() { }
    }

    internal class GaussianErrorTermForHeight : GaussianErrorTerm
    {
        public GaussianErrorTermForHeight(GaussianErrorTermList.IIndexableErrorTerm caller, double normalizedValue, double observedValue) : base(caller, normalizedValue)
        {
            this.value = observedValue;
        }
    }

    ///
    /// <summary>
    ///The HDRelationshipPredictor class is the basic class for all HD relationships 
    ///based on linear mixed-effects modelling.
    /// </summary>
    public abstract class HDRelationshipPredictor<Stand, Tree> : REpiceaPredictor
            where Stand : IMonteCarloSimulationCompliantObject
            where Tree : IHDRelationshipTree {

        protected readonly Dictionary<string, double> observedHeights;

        ///
        /// <summary>
        /// Preferred constructor.
        /// </summary>
        protected HDRelationshipPredictor(bool isVariabilityEnabledEnabled) : this(isVariabilityEnabledEnabled, isVariabilityEnabledEnabled, isVariabilityEnabledEnabled) {
        }

        ///
        /// <summary>
        /// Second constructor for greater flexibility
        /// </summary>
        protected HDRelationshipPredictor(bool isParameterVariabilityEnabled, bool isRandomEffectVariabilityEnabled, bool isResidualErrorVariabilityEnabled) :
                        base(isParameterVariabilityEnabled, isRandomEffectVariabilityEnabled, isResidualErrorVariabilityEnabled)
        {
            observedHeights = new Dictionary<string, double>();
        }

        public double predictHeightM(Stand stand, Tree tree)
        {
            if (!HasSubjectBeenTestedForBlups(stand))
            {
                PredictHeightRandomEffects(stand);  // this method now deals with the blups and the residual error so that if observed height is greater than 1.3 m there is no need to avoid predicting the height
            }
            RegressionElements regElement = FixedEffectsPrediction(stand, tree, GetParametersForThisRealization(stand));
            double predictedHeight = regElement.fixedPred;
            predictedHeight += BlupImplementation(stand, regElement);
            predictedHeight += ResidualImplementation(tree, predictedHeight);
            if (predictedHeight < 1.3)
            {
                predictedHeight = 1.3;
            }
            return predictedHeight;
        }

        ///
        /// <summary>
        /// Accounts for the random effects in the predictions if the random effect variability is enabled. 
        /// </summary>
        /// <returns>
        /// A simulated random effect (double)
        /// </returns>
        protected double BlupImplementation(Stand stand, RegressionElements regElement)
        {
            Matrix randomEffects = GetRandomEffectsForThisSubject(stand);
            return regElement.vectorZ.Multiply(randomEffects).GetValueAt(0, 0);
        }



        ///
        ///<summary>
        /// Record a normalized residuals into the simulatedResidualError member which is
        /// located in the REpiceaPredictor class. The method asks the date from the IHeightableTree
        /// instance in order to put the normalized residual at the proper location in the vector of residuals.
        ///</summary>
        protected void SetSpecificResiduals(Tree tree, GaussianErrorTerm errorTerm)
        {
            GaussianErrorTermList list = GetGaussianErrorTerms(tree);
            if (!list.GetDistanceIndex().Contains(tree.GetErrorTermIndex()))
            {       // we add the GaussianErrorTerm only if it is not already in the list
                list.Add(errorTerm);
            }
        }

        ///
        /// <summary>
        ///Accounts for a random deviate if the residual variability is enabled.Otherwise, it returns 0d. 
        /// </summary>
        /// <returns>
        /// A simulated residual(double)
        /// </returns>
        protected double ResidualImplementation(Tree tree, double predictedHeightWithoutResidual)
        {
            double residualForThisPrediction = 0d;
            if (WasThisTreeInitiallyMeasured(tree) && !DoesThisSubjectHaveResidualErrorTerm(tree))
            {   // means the height has been observed but its residual has not been calculated yet
                double variance = GetDefaultResidualError(GetErrorGroup(tree)).GetVariance().GetValueAt(0, 0);
                double diff = observedHeights[tree.GetSubjectId()] - predictedHeightWithoutResidual;
                double dNormResidual = diff / Math.Pow(variance, 0.5);
                GaussianErrorTerm errorTerm = new GaussianErrorTermForHeight(tree, dNormResidual, diff);
                SetSpecificResiduals(tree, errorTerm);  // the residual is set in the simulatedResidualError member
            }
            if (isResidualVariabilityEnabled)
            {
                Matrix residuals = GetResidualErrorForThisSubject(tree, GetErrorGroup(tree));
                int index = GetGaussianErrorTerms(tree).GetDistanceIndex().IndexOf(tree.GetErrorTermIndex());
                residualForThisPrediction = residuals.GetValueAt(index, 0);
            }
            else
            {
                if (DoesThisSubjectHaveResidualErrorTerm(tree))
                {       // means that height was initially measured
                    SetSpecificResiduals(tree, new GaussianErrorTerm(tree, 0d));
                    GaussianErrorTermList list = GetGaussianErrorTerms(tree);
                    Matrix meanResiduals = GetDefaultResidualError(GetErrorGroup(tree)).GetMean(list);
                    residualForThisPrediction = meanResiduals.GetValueAt(meanResiduals.m_iRows - 1, 0);
                }
            }
            return residualForThisPrediction;
        }

        protected bool WasThisTreeInitiallyMeasured(Tree tree) {
            return observedHeights.ContainsKey(tree.GetSubjectId());
        }

        ///
        /// <summary>
        /// Compute the best linear unbiased predictors of the random effects
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void PredictHeightRandomEffects(Stand stand)
        {
            if (!HasSubjectBeenTestedForBlups(stand))
            {
                Matrix matGbck = GetDefaultRandomEffects(HierarchicalLevel.PLOT).GetVariance();
                RegressionElements regElement;
                List<IHDRelationshipTree> heightableTrees = new List<IHDRelationshipTree>(); // put all the trees for which the height is available in a List

                Matrix defaultBeta = GetParameterEstimates().GetMean();     // at this point the mean only contains the fixed effects
               Matrix omega = GetParameterEstimates().GetVariance();

                ICollection trees = GetTreesFromStand(stand);
                heightableTrees.Clear();
                if (trees != null && trees.Count > 0)
                {
                    foreach (Object tree in trees) {
					    if (tree is IHDRelationshipTree) {
						    double height = ((IHDRelationshipTree) tree).GetHeightM();
                            if (height > 1.3)
                            {
                                heightableTrees.Add((IHDRelationshipTree)tree);
                            }
    					}
	    			}
		    	}
			    if (heightableTrees.Count > 0)
                {
                    // matrices for the blup calculation
                    List<int> trueParameterIndices = GetParameterEstimates().GetTrueParameterIndices();
                    int nbParameters = trueParameterIndices.Count;
                    int nbObs = heightableTrees.Count;
                    Matrix matZ_i = new Matrix(nbObs, matGbck.m_iRows);     // design matrix for random effects 
                    Matrix matR_i = new Matrix(nbObs, nbObs);                   // within-tree variance-covariance matrix  
                    Matrix matX_i = new Matrix(nbObs, nbParameters);                    // within-tree variance-covariance matrix  
                    Matrix res_i = new Matrix(nbObs, 1);                        // vector of residuals

                    for (int i = 0; i < nbObs; i++)
                    {
                        Tree t = (Tree)heightableTrees[i];
                        double height = t.GetHeightM();
                        regElement = FixedEffectsPrediction(stand, t, defaultBeta);
                        matX_i.SetSubMatrix(oXVector.GetSubMatrix(DefaultZeroIndex, trueParameterIndices), i, 0);
                        matZ_i.SetSubMatrix(regElement.vectorZ, i, 0);
                        double variance = GetDefaultResidualError(GetErrorGroup(t)).GetVariance().GetValueAt(0, 0);
                        matR_i.SetValueAt(i, i, variance);
                        double residual = height - regElement.fixedPred;
                        res_i.SetValueAt(i, 0, residual);
                    }
                    Matrix matV_i = matZ_i.Multiply(matGbck).Multiply(matZ_i.Transpose()).Add(matR_i);
                    Matrix invV_i = matV_i.GetInverseMatrix();
                    Matrix blups_i = matGbck.Multiply(matZ_i.Transpose()).Multiply(invV_i).Multiply(res_i);

                    SymmetricMatrix newMatG_i = null;

                    if (isRandomEffectsVariabilityEnabled)
                    {
                        Matrix matP = invV_i.Subtract(invV_i.Multiply(matX_i).Multiply(omega).Multiply(matX_i.Transpose()).Multiply(invV_i));
                        newMatG_i = SymmetricMatrix.ConvertToSymmetricIfPossible(matGbck.Subtract(matGbck.Multiply(matZ_i.Transpose()).Multiply(matP).Multiply(matZ_i).Multiply(matGbck)));
                    }

                    SetBlupsForThisSubject(stand, new GaussianEstimate(blups_i, newMatG_i));

                    foreach (IHDRelationshipTree t in heightableTrees)
                    {
                        observedHeights[t.GetSubjectId()] = t.GetHeightM();
                    }
                }
                RecordSubjectTestedForBlups(stand);
            }
        }

        protected Enum GetErrorGroup(Tree tree)
        {
            Enum errorGroup = tree.GetHDRelationshipTreeErrorGroup();
            if (errorGroup == null)
            {
                return ErrorTermGroup.Default;
            }
            else
            {
                return errorGroup;
            }
        }

        ///
        /// <summary>
        /// Select the trees from which the blups must be calculated.
        /// </summary>
        /// <returns>
        /// a Collection of Tree instances
        /// </returns>
        protected abstract Collection<Tree> GetTreesFromStand(Stand stand);

        ///
        /// <summary>
        /// Compute the fixed effect prediction and put the prediction, the Z vector,
        /// and the species name into m_oRegressionOutput member.The method applies in any cases no matter
        /// it is deterministic or stochastic. IMPORTANT: This method must be synchronized!!!!
        /// </summary>
        /// <returns>
        /// a RegressionElements instance
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract RegressionElements FixedEffectsPrediction(Stand stand, Tree t, Matrix beta);
        	
	
    }

}
