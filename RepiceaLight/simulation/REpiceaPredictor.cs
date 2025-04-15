using REpiceaLight.math;
using REpiceaLight.simulation;
using REpiceaLight.stats.distributions;
using REpiceaLight.stats.estimates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.simulation.REpiceaPredictor;
using static REpiceaLight.simulation.REpiceaPredictorEvent;
using static REpiceaLight.stats.distributions.GaussianErrorTermList;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.simulation
{
    public abstract class REpiceaPredictor : SensitivityAnalysisParameter<ModelParameterEstimates>
    {


        protected static readonly List<int> DefaultZeroIndex = new();
        static REpiceaPredictor()
        {
            DefaultZeroIndex.Add(0);
        }

        /**
         * This class creates a fake subject for interval random effects nested in the plots. 
         * @author Mathieu Fortin - November 2016
         */
        protected class IntervalNestedInPlotDefinition : IMonteCarloSimulationCompliantObject
        {


            private readonly int monteCarloRealizationID;
            private readonly string subjectID;

            public IntervalNestedInPlotDefinition(IMonteCarloSimulationCompliantObject stand, int date)
            {
                monteCarloRealizationID = stand.GetMonteCarloRealizationId();
                subjectID = GetSubjectID(stand, date);
            }


            public string GetSubjectId()
            {
                return subjectID;
            }

            public HierarchicalLevel GetHierarchicalLevel()
            {
                return HierarchicalLevel.INTERVAL_NESTED_IN_PLOT;
            }


            public int GetMonteCarloRealizationId()
            {
                return monteCarloRealizationID;
            }

            internal static string GetSubjectID(IMonteCarloSimulationCompliantObject stand, int date)
            {
                return stand.GetSubjectId() + "_" + date;
            }
        }

        /**
         * This class creates a fake subject for cruise line random effects.
         * @author Mathieu Fortin - April 2017
         */
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

        //	protected REpiceaRandom random = new REpiceaRandom();


        /**
         * General constructor for all combinations of uncertainty sources.
         * @param isParametersVariabilityEnabled a boolean that enables the variability at the parameter level
         * @param isRandomEffectsVariabilityEnabled a boolean that enables the variability at the random effect level
         * @param isResidualVariabilityEnabled a boolean that enables the variability at the tree level
         */
        protected REpiceaPredictor(bool isParametersVariabilityEnabled,
                bool isRandomEffectsVariabilityEnabled,
                bool isResidualVariabilityEnabled) : base(isParametersVariabilityEnabled)
        {
            this.isRandomEffectsVariabilityEnabled = isRandomEffectsVariabilityEnabled;
            this.isResidualVariabilityEnabled = isResidualVariabilityEnabled;

            defaultRandomEffects = new();
            blupsRandomEffects = new();
            subjectTestedForBlups = new();

            simulatedRandomEffects = new();
            simulatedResidualError = new();

            intervalLists = new();
            cruiseLineMap = new();

            defaultResidualError = new();

            listeners = new();
        }

        /**
         * This method reads all the parameters in .csv files and stores the estimates into members defaultBeta, defaultResidualError,
         * and defaultRandomEffects.
         */
        protected abstract void Init();

        protected Dictionary<string, GaussianEstimate> GetDefaultRandomEffects()
        {
            return defaultRandomEffects;
        }

        protected void SetParameterEstimates(ModelParameterEstimates gaussianEstimate)
        {
            //		super.setParameterEstimates(new ModelParameterEstimates(gaussianEstimate, this));
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

        protected GaussianErrorTermEstimate GetDefaultResidualError(Enum enumVar)
        {
            return defaultResidualError[enumVar];
        }



        /**
         * This method checks if the interval definition is available for the stand at that date. If it is, it returns the
         * instance. Otherwise, it creates a new interval definition.
         * @param stand A MonteCarloSimulationCompliantObject that designates the stand
         * @param date an Integer
         * @return an IntervalDefinition instance
         */
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

        /**
         * This method checks if a cruise line exists for this plot
         * @param cruiseLineID the id of the cruise line
         * @param stand a MonteCarloSimulationCompliantObject instance
         * @return a CruiseLine instance
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected CruiseLine GetCruiseLineForThisSubject(string cruiseLineID, IMonteCarloSimulationCompliantObject stand)
        {
            string cruiseLineIDPlusMCRealization = cruiseLineID + "_" + stand.GetMonteCarloRealizationId();
            if (!cruiseLineMap.ContainsKey(cruiseLineIDPlusMCRealization))
                cruiseLineMap[cruiseLineIDPlusMCRealization] = new CruiseLine(cruiseLineID, stand);

            return cruiseLineMap[cruiseLineIDPlusMCRealization];
        }

        /**
         * This method calls the setSpecificParametersDeviateForThisRealization method if the parameter variability is enabled and returns 
         * a realization-specific simulated vector of model parameters. Otherwise it returns a default vector (beta). Note that the simulated
         * parameters are related to the Monte Carlo realization. For instance, all subject in a given Monte Carlo realization will have the
         * same simulation parameters. 
         * @param subject a subject that implements the MonteCarloSimulationCompliantObject interface
         * @return a vector of parameters
         */
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


        /**
         * This method generates a subject-specific random effects vector using matrix G.
         * @param subject a MonteCarloSimulationCompliantObject instance
         */
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


        /**
         * This method simulates random deviates from an estimate and stores them in the simulatedRandomEffects
         * member.
         * @param subject a MonteCarloSimulationCompliantObject instance
         * @param randomEffectsEstimate the estimate from which the random deviates are generated
         * @return the random deviates as a Matrix instance (a copy of it)
         */
        protected Matrix SimulateDeviatesForRandomEffectsOfThisSubject(IMonteCarloSimulationCompliantObject subject, GaussianEstimate randomEffectsEstimate)
        {
            Matrix randomDeviates = randomEffectsEstimate.GetRandomDeviate();
            SetDeviatesForRandomEffectsOfThisSubject(subject, randomDeviates);
            return randomDeviates.Clone();
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

        /**
         * This method calls the setSpecificPlotRandomEffectsForThisStand method if the random effects variability is enabled and returns 
         * a stand-specific simulated vector of random effects. Otherwise it returns a default vector (all elements set to 0).
         * @param subject a MonteCarloSimulationCompliantObject object
         * @return a Matrix object
         */
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



        /**
         * This method returns the residual error or the vector of residual errors associated with the subjectId.
         * If the subject parameter is entered as null, the method assumes there is no need to store the simulated
         * error terms in the simulatedResidualError map. This feature is useful if the residual error terms are 
         * identically and independently distributed.
         * @param subject a MonteCarloSimulationCompliantObject instance
         * @param group an Enum that defines the group in case of different error term specifications
         * @return a Matrix instance
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected Matrix GetResidualErrorForThisSubject(IMonteCarloSimulationCompliantObject? subject, Enum group)
        {
            if (isResidualVariabilityEnabled)
            {
                if (subject != null && subject is IIndexableErrorTerm && defaultResidualError[group].GetDistribution().IsStructured())
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


        /**
         * This method returns the residual error under the assumption of iid and that the error is unique for all groups. 
         * See the getResidualErrorForThisSubject method.
         * @return a Matrix instance
         */
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
	
	
	/**
	 * This method adds the listener instance to the list of listeners.
	 * @param listener a ModelBasedSimulatorListener listener
	 */
        public void AddModelBasedSimulatorListener(IREpiceaPredictorListener listener)
        {
            if (!listeners.ContainsKey(listener))
                listeners[listener] = 0;
        }

        /**
         * This method removes the listener instance from the list of listeners.
         * @param listener a ModelBasedSimulatorListener listener
         */
        public void RemoveModelBasedSimulatorListener(IREpiceaPredictorListener listener)
        {
            int i;
            listeners.Remove(listener, out i);
        }

        //	/**
        //	 * This method enables the recording of the random deviates. By default, this option is set to true.
        //	 * It can be desirable to set this option to false when running large stochastic simulations.
        //	 * @param rememberRandomDeviates a boolean
        //	 */
        //	public void setRememberRandomDeviates(boolean rememberRandomDeviates) {
        //		this.rememberRandomDeviates = rememberRandomDeviates;
        //	}

        protected bool DoBlupsExistForThisSubject(IMonteCarloSimulationCompliantObject subject)
        {
            return GetBlupsForThisSubject(subject) != null;
        }


        /**
         * This method returns the blups for the subject or nothing if there is no blups for this subject
         * @param subject a MonteCarloSimulationCompliantObject instance
         * @return an Estimate instance or null
         */
        protected GaussianEstimate? GetBlupsForThisSubject(IMonteCarloSimulationCompliantObject subject)
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
                blupsRandomEffects[hierarchicalName] = new();

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
                subjectTestedForBlups[hierarchicalName] = new();

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
