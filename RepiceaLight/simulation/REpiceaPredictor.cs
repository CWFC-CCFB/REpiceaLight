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
using System.Runtime.CompilerServices;
using static REpiceaLight.simulation.REpiceaPredictorEvent;
using static REpiceaLight.stats.distributions.GaussianErrorTermList;

namespace REpiceaLight.simulation
{
    /// <summary>
    /// An abstract class for any statistical model to be implemented. It provides all the features
    /// to run a model in either stochastic or deterministic mode.
    /// </summary>
    public abstract class REpiceaPredictor : SensitivityAnalysisParameter<ModelParameterEstimates>
    {


        protected static readonly List<int> DefaultZeroIndex = new List<int>();
        static REpiceaPredictor()
        {
            DefaultZeroIndex.Add(0);
        }

        /// <summary>
        /// An inner class to handle interval random effects nested in the plots
        /// </summary>
        protected class IntervalNestedInPlotDefinition : IMonteCarloSimulationCompliantObject
        {

            private readonly int monteCarloRealizationID;
            private readonly string subjectID;

            public IntervalNestedInPlotDefinition(IMonteCarloSimulationCompliantObject stand, int date)
            {
                monteCarloRealizationID = stand.GetMonteCarloRealizationId();
                subjectID = GetSubjectID(stand, date);
            }

            public string GetSubjectId() { return subjectID; }

            public HierarchicalLevel GetHierarchicalLevel() { return HierarchicalLevel.INTERVAL_NESTED_IN_PLOT; }

            public int GetMonteCarloRealizationId() { return monteCarloRealizationID; }
            
            internal static string GetSubjectID(IMonteCarloSimulationCompliantObject stand, int date) { return stand.GetSubjectId() + "_" + date; }
        }

        /// <summary>
        /// An inner class to handle cruise line random effects.
        /// </summary>
        protected class CruiseLine : IMonteCarloSimulationCompliantObject
        {

            private readonly string subjectID;
            private readonly int monteCarloRealizationID;

            public CruiseLine(string subjectID, IMonteCarloSimulationCompliantObject subject)
            {
                this.subjectID = subjectID;
                monteCarloRealizationID = subject.GetMonteCarloRealizationId();
            }

            public string GetSubjectId() { return subjectID; }

            public HierarchicalLevel GetHierarchicalLevel() { return HierarchicalLevel.CRUISE_LINE; }

            public int GetMonteCarloRealizationId() { return monteCarloRealizationID; }
        }

        public enum ErrorTermGroup { Default }

        protected readonly System.Collections.Concurrent.ConcurrentDictionary<IREpiceaPredictorListener, int> listeners;


        private readonly Dictionary<string, CruiseLine> cruiseLineMap;
        private readonly Dictionary<string, IntervalNestedInPlotDefinition> intervalLists;

        // set by the constructor
        protected readonly bool isRandomEffectsVariabilityEnabled;
        protected readonly bool isResidualVariabilityEnabled;

        protected Matrix oXVector;

        readonly Dictionary<string, GaussianEstimate> defaultRandomEffects;
        readonly Dictionary<string, Dictionary<string, GaussianEstimate>> blupsRandomEffects; // key1: hierarchical level, key2: subject id
        readonly Dictionary<string, List<string>> subjectTestedForBlups; // key: hierarchical level

        private readonly Dictionary<string, Dictionary<string, Matrix>> simulatedRandomEffects;  // refers to the subject + realization ids

        private readonly Dictionary<Enum, GaussianErrorTermEstimate> defaultResidualError;
        readonly Dictionary<string, GaussianErrorTermList> simulatedResidualError;        // refers to the subject + realization ids

        /// <summary>
        /// General constructor for all combinations of uncertainty sources.
        /// </summary>
        /// <param name="isParametersVariabilityEnabled">a boolean that enables the variability at the parameter level</param>
        /// <param name="isRandomEffectsVariabilityEnabled">a boolean that enables the variability at the random effect level</param>
        /// <param name="isResidualVariabilityEnabled">a boolean that enables the variability at the tree level</param>
        protected REpiceaPredictor(bool isParametersVariabilityEnabled,
                bool isRandomEffectsVariabilityEnabled,
                bool isResidualVariabilityEnabled) : base(isParametersVariabilityEnabled)
        {
            this.isRandomEffectsVariabilityEnabled = isRandomEffectsVariabilityEnabled;
            this.isResidualVariabilityEnabled = isResidualVariabilityEnabled;

            defaultRandomEffects = new Dictionary<string, GaussianEstimate>();
            blupsRandomEffects = new Dictionary<string, Dictionary<string, GaussianEstimate>>();
            subjectTestedForBlups = new Dictionary<string, List<string>>();

            simulatedRandomEffects = new Dictionary<string, Dictionary<string, Matrix>>();
            simulatedResidualError = new Dictionary<string, GaussianErrorTermList>();

            intervalLists = new Dictionary<string, IntervalNestedInPlotDefinition>();
            cruiseLineMap = new Dictionary<string, CruiseLine>();

            defaultResidualError = new Dictionary<Enum, GaussianErrorTermEstimate>();

            listeners = new System.Collections.Concurrent.ConcurrentDictionary<IREpiceaPredictorListener, int>();
        }

        protected abstract void Init();

        protected Dictionary<string, GaussianEstimate> GetDefaultRandomEffects() { return defaultRandomEffects; }

        protected override void SetParameterEstimates(ModelParameterEstimates gaussianEstimate) 
        {
            base.SetParameterEstimates(gaussianEstimate);
            FireModelBasedSimulatorEvent(new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.DEFAULT_BETA_JUST_SET, null, GetParameterEstimates(), this));
        }

        protected void SetDefaultRandomEffects(HierarchicalLevel level, GaussianEstimate newEstimate)
        {
            GaussianEstimate formerEstimate = defaultRandomEffects[level.GetName()];
            defaultRandomEffects[level.GetName()] = newEstimate;
            FireModelBasedSimulatorEvent(new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.DEFAULT_RANDOM_EFFECT_AT_THIS_LEVEL_JUST_SET, null, new object[] { level, formerEstimate, newEstimate }, this));
        }

        protected GaussianEstimate GetDefaultRandomEffects(HierarchicalLevel level)
        {
            return defaultRandomEffects[level.GetName()];
        }

        protected void SetDefaultResidualError(Enum enumVar, GaussianErrorTermEstimate estimate)
        {
            defaultResidualError[enumVar] = estimate;
            FireModelBasedSimulatorEvent(new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.DEFAULT_RESIDUAL_ERROR_JUST_SET, null, new object[] { enumVar, estimate }, this));
        }

        protected GaussianErrorTermEstimate GetDefaultResidualError(Enum enumVar) { return defaultResidualError[enumVar]; }

        /// <summary>
        /// Check if the interval definition is available for the stand at that date. 
        /// If it is, it returns the instance.Otherwise, it creates a new interval definition.
        /// </summary>
        /// <param name="stand">a MonteCarloSimulationCompliantObject designating the plot</param>
        /// <param name="date">an Integer</param>
        /// <returns>an IntervalDefinition instance</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected IntervalNestedInPlotDefinition GetIntervalNestedInPlotDefinition(IMonteCarloSimulationCompliantObject stand, int date)
        {
            string subjectID = IntervalNestedInPlotDefinition.GetSubjectID(stand, date);
            string intervalID = GetSubjectPlusMonteCarloSpecificId(subjectID, stand.GetMonteCarloRealizationId());
            IntervalNestedInPlotDefinition intDef = intervalLists[intervalID];
            if (intDef == null)
            {
                intDef = new IntervalNestedInPlotDefinition(stand, date);
                intervalLists[GetSubjectPlusMonteCarloSpecificId(intDef)] = intDef;
            }
            return intDef;
        }

        /// <summary>Provide the CruiseLine instance for a plot.</summary>
        /// <param name="cruiseLineID">the id of the cruise line</param>
        /// <param name="stand">a MonteCarloSimulationCompliantObject instance</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected CruiseLine GetCruiseLineForThisSubject(string cruiseLineID, IMonteCarloSimulationCompliantObject stand)
        {
            string cruiseLineIDPlusMCRealization = cruiseLineID + "_" + stand.GetMonteCarloRealizationId();
            if (!cruiseLineMap.ContainsKey(cruiseLineIDPlusMCRealization))
                cruiseLineMap[cruiseLineIDPlusMCRealization] = new CruiseLine(cruiseLineID, stand);

            return cruiseLineMap[cruiseLineIDPlusMCRealization];
        }

        /// <summary>
        /// Provide random deviates of the parameter estimates in cases of stochastic simulation or the
        /// mean parameter estimates in cases of deterministic simulation.
        /// </summary>
        /// <param name="subject"> a subject that implements the MonteCarloSimulationCompliantObject interface</param>
        /// <returns>a vector of parameters</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override Matrix GetParametersForThisRealization(IMonteCarloSimulationCompliantObject subject)
        {
            if (isParametersVariabilityEnabled)
            {
                if (!simulatedParameters.ContainsKey(subject.GetMonteCarloRealizationId()))
                    simulatedParameters[subject.GetMonteCarloRealizationId()] = GetParameterEstimates().GetRandomDeviate();

                return simulatedParameters[subject.GetMonteCarloRealizationId()];
            }
            else
                return GetParameterEstimates().GetMean();
        }


        private void SetSpecificRandomEffectsForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            HierarchicalLevel subjectLevel = subject.GetHierarchicalLevel();

            Matrix randomDeviates;
            GaussianEstimate originalRandomEffects;
            if (DoBlupsExistForThisSubject(subject))
                SimulateDeviatesForRandomEffectsOfThisSubject(subject, GetBlupsForThisSubject(subject));
            else
            {
                randomDeviates = SimulateDeviatesForRandomEffectsOfThisSubject(subject, defaultRandomEffects[subjectLevel.GetName()]);
                originalRandomEffects = GetDefaultRandomEffects(subjectLevel);
                FireRandomEffectDeviateGeneratedEvent(subject, originalRandomEffects, randomDeviates);
            }
        }

        protected void FireRandomEffectDeviateGeneratedEvent(IMonteCarloSimulationCompliantObject subject, GaussianEstimate originalRandomEffects, Matrix randomDeviates)
        {
            object newValue = new object[] { subject, originalRandomEffects, randomDeviates.Clone() };

            REpiceaPredictorEvent ev = new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.RANDOM_EFFECT_DEVIATE_JUST_GENERATED,
                    null,
                    newValue,
                    this);
            FireModelBasedSimulatorEvent(ev);
        }

        protected Matrix SimulateDeviatesForRandomEffectsOfThisSubject(IMonteCarloSimulationCompliantObject subject, GaussianEstimate randomEffectsEstimate)
        {
            Matrix randomDeviates = randomEffectsEstimate.GetRandomDeviate();
            SetDeviatesForRandomEffectsOfThisSubject(subject, randomDeviates);
            return (Matrix) randomDeviates.Clone();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void SetDeviatesForRandomEffectsOfThisSubject(IMonteCarloSimulationCompliantObject subject, Matrix randomDeviates)
        {
            HierarchicalLevel subjectLevel = subject.GetHierarchicalLevel();
            if (!simulatedRandomEffects.ContainsKey(subjectLevel.GetName()))
                simulatedRandomEffects[subjectLevel.GetName()] = new Dictionary<string, Matrix>();

            Dictionary<string, Matrix> randomEffectsMap = simulatedRandomEffects[subjectLevel.GetName()];
            randomEffectsMap[GetSubjectPlusMonteCarloSpecificId(subject)] = randomDeviates;
        }

        protected string GetSubjectPlusMonteCarloSpecificId(IMonteCarloSimulationCompliantObject obj)
        {
            return GetSubjectPlusMonteCarloSpecificId(obj.GetSubjectId(), obj.GetMonteCarloRealizationId());
        }

        internal static string GetSubjectPlusMonteCarloSpecificId(string subjectID, int monteCarloRealizationID)
        {
            return subjectID + "_" + monteCarloRealizationID;
        }

        /// <summary>
        /// Provide the random effect for this subject. 
        /// </summary>
        /// <param name="subject">a MonteCarloSimulationCompliantObject object</param>
        /// <returns>a Matrix object</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected Matrix GetRandomEffectsForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            HierarchicalLevel subjectLevel = subject.GetHierarchicalLevel();
            if (isRandomEffectsVariabilityEnabled)
            {
                if (!DoRandomDeviatesExistForThisSubject(subject))
                    SetSpecificRandomEffectsForThisSubject(subject);

                return simulatedRandomEffects[subjectLevel.GetName()][GetSubjectPlusMonteCarloSpecificId(subject)];
            }
            else
            {
                GaussianEstimate blups = GetBlupsForThisSubject(subject);
                if (blups != null)
                    return blups.GetMean();
                else
                    return defaultRandomEffects[subjectLevel.GetName()].GetMean();
            }
        }

        protected bool DoRandomDeviatesExistForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            HierarchicalLevel subjectLevel = subject.GetHierarchicalLevel();
            return simulatedRandomEffects[subjectLevel.GetName()] != null && simulatedRandomEffects[subjectLevel.GetName()].ContainsKey(GetSubjectPlusMonteCarloSpecificId(subject));
        }

        /// <summary>
        /// Provide the residual error (stochastic) or its expectation in cases of deterministic simulation.
        /// </summary>
        /// <param name="subject">a MonteCarloSimulationCompliantObject instance</param>
        /// <param name="group">an Enum that defines the group in case of different error term specifications</param>
        /// <returns>a Matrix instance</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected Matrix GetResidualErrorForThisSubject(IMonteCarloSimulationCompliantObject subject, Enum group)
        {
            if (isResidualVariabilityEnabled)
            {
                if (subject != null && subject is IIndexableErrorTerm && ((CenteredGaussianDistribution) defaultResidualError[group].GetDistribution()).IsStructured())
                {
                    IIndexableErrorTerm indexable = (IIndexableErrorTerm)subject;
                    GaussianErrorTermList list = GetGaussianErrorTerms(subject);
                    if (!list.GetDistanceIndex().Contains(indexable.GetErrorTermIndex()))
                        list.Add(new GaussianErrorTerm(indexable));

                    Matrix randomDeviate = defaultResidualError[group].GetRandomDeviate(list);
                    FireModelBasedSimulatorEvent(new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.RESIDUAL_ERROR_DEVIATE_JUST_GENERATED, null, new object[] { subject, group, randomDeviate.Clone() }, this));
                    return randomDeviate;
                }
                else
                {
                    Matrix randomDeviate = defaultResidualError[group].GetRandomDeviate();
                    FireModelBasedSimulatorEvent(new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.RESIDUAL_ERROR_DEVIATE_JUST_GENERATED, null, new object[] { subject, group, randomDeviate.Clone() }, this));
                    return randomDeviate;
                }
            }
            else
                return defaultResidualError[group].GetMean();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected GaussianErrorTermList GetGaussianErrorTerms(IMonteCarloSimulationCompliantObject subject)
        {
            if (!DoesThisSubjectHaveResidualErrorTerm(subject))
            {       // the simulated parameters remain constant within the same Monte Carlo iteration
                simulatedResidualError[GetSubjectPlusMonteCarloSpecificId(subject)] = new GaussianErrorTermList();
            }
            GaussianErrorTermList list = simulatedResidualError[GetSubjectPlusMonteCarloSpecificId(subject)];
            return list;
        }

        protected bool DoesThisSubjectHaveResidualErrorTerm(IMonteCarloSimulationCompliantObject subject)
        {
            return simulatedResidualError.ContainsKey(GetSubjectPlusMonteCarloSpecificId(subject));
        }


        protected Matrix GetResidualError()
        {
            return GetResidualErrorForThisSubject(null, ErrorTermGroup.Default);
        }

        protected void FireModelBasedSimulatorEvent(REpiceaPredictorEvent ev)
        {
            foreach (IREpiceaPredictorListener listener in listeners.Keys) {
                listener.ModelBasedSimulatorDidThis(ev);
            }
	    }

        /// <summary>
        /// Add a listener.
        /// </summary>
        /// <param name="listener">an IREpiceaPredictorListener instance</param>        
        public void AddModelBasedSimulatorListener(IREpiceaPredictorListener listener)
        {
            if (!listeners.ContainsKey(listener))
                listeners[listener] = 0;
        }

        /// <summary>
        /// Remove a listener
        /// </summary>
        /// <param name="listener">an IREpiceaPredictorListener instance</param>
        public void RemoveModelBasedSimulatorListener(IREpiceaPredictorListener listener)
        {
            listeners.TryRemove(listener, out int i);
        }

        protected bool DoBlupsExistForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            return GetBlupsForThisSubject(subject) != null;
        }

        /// <summary>
        /// Provide the blups for the subject.
        /// </summary>
        /// <param name="subject">a MonteCarloSimulationCompliantObject instance</param>
        /// <returns>an GaussianEstimate instance or null if the subject has no blups</returns>
        protected GaussianEstimate GetBlupsForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            string hierarchicalName = subject.GetHierarchicalLevel().GetName();
            if (blupsRandomEffects.ContainsKey(hierarchicalName))
            {
                if (blupsRandomEffects[hierarchicalName].ContainsKey(subject.GetSubjectId()))
                    return blupsRandomEffects[hierarchicalName][subject.GetSubjectId()];
            }
            return null;
        }

        protected void SetBlupsForThisSubject(IMonteCarloSimulationCompliantObject subject, GaussianEstimate blups)
        {
            string hierarchicalName = subject.GetHierarchicalLevel().GetName();
            if (!blupsRandomEffects.ContainsKey(hierarchicalName))
                blupsRandomEffects[hierarchicalName] = new Dictionary<string, GaussianEstimate>();

            blupsRandomEffects[hierarchicalName][subject.GetSubjectId()] = blups;

            REpiceaPredictorEvent ev = new REpiceaPredictorEvent(ModelBasedSimulatorEventProperty.BLUPS_JUST_SET,
                null,
                new object[] { defaultRandomEffects[subject.GetHierarchicalLevel().GetName()], subject },
                this);
            FireModelBasedSimulatorEvent(ev);
        }

        protected void RecordSubjectTestedForBlups(IMonteCarloSimulationCompliantObject subject)
        {
            string hierarchicalName = subject.GetHierarchicalLevel().GetName();
            if (!subjectTestedForBlups.ContainsKey(hierarchicalName))
                subjectTestedForBlups[hierarchicalName] = new List<string>();

            if (subjectTestedForBlups[hierarchicalName].Contains(subject.GetSubjectId()))
                throw new InvalidOperationException("The subject has already been tested for blups!");
            else
                subjectTestedForBlups[hierarchicalName].Add(subject.GetSubjectId());
        }

        protected bool HasSubjectBeenTestedForBlups(IMonteCarloSimulationCompliantObject subject)
        {
            string hierarchicalName = subject.GetHierarchicalLevel().GetName();
            if (subjectTestedForBlups.ContainsKey(hierarchicalName))
                return subjectTestedForBlups[hierarchicalName].Contains(subject.GetSubjectId());
            else
                return false;
        }

        public override bool IsStochastic()
        {
            return base.IsStochastic() || isRandomEffectsVariabilityEnabled || isResidualVariabilityEnabled;
        }

    }

}
