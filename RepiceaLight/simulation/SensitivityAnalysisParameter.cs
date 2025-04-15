using REpiceaLight.math;
using REpiceaLight.simulation.covariateproviders.samplelevel;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.simulation
{
    public abstract class SensitivityAnalysisParameter<E> : IStochasticImplementation where E : IEstimate
    {

        internal readonly Dictionary<int, Matrix> simulatedParameters;     // refers to the realization id only
        private E parameterEstimates;
        protected bool isParametersVariabilityEnabled;

        protected SensitivityAnalysisParameter(bool isParametersVariabilityEnabled)
        {
            this.isParametersVariabilityEnabled = isParametersVariabilityEnabled;
            simulatedParameters = new();
        }

        protected void SetParameterEstimates(E estimate)
        {
            this.parameterEstimates = estimate;
        }

        protected E GetParameterEstimates() { return parameterEstimates; }


        /**
         * This method calls the setSpecificParametersDeviateForThisRealization method if the parameter variability is enabled and returns 
         * a realization-specific simulated vector of model parameters. Otherwise it returns a default vector (beta). Note that the simulated
         * parameters are related to the Monte Carlo realization. For instance, all subject in a given Monte Carlo realization will have the
         * same simulation parameters. 
         * @param subject a subject that implements the MonteCarloSimulationCompliantObject interface
         * @return a vector of parameters
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual Matrix GetParametersForThisRealization(IMonteCarloSimulationCompliantObject subject)
        {
            if (isParametersVariabilityEnabled)
            {
                string subjectPlusMonteCarloId = REpiceaPredictor.GetSubjectPlusMonteCarloSpecificId(subject.GetSubjectId(), subject.GetMonteCarloRealizationId());
                int hashCodeSubjectId = subjectPlusMonteCarloId.GetHashCode();
                if (!simulatedParameters.ContainsKey(hashCodeSubjectId))
                {       // the simulated parameters remain constant within the same Monte Carlo iteration
                    Matrix randomDeviates = GetParameterEstimates().GetRandomDeviate();
                    simulatedParameters[hashCodeSubjectId] = randomDeviates;
                }
                return simulatedParameters[hashCodeSubjectId];
            }
            else
            {
                return GetParameterEstimates().GetMean();
            }
        }

        public virtual bool IsStochastic() { return isParametersVariabilityEnabled; }

    }

}
