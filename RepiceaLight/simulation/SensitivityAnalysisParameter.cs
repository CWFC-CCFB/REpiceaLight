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
using REpiceaLight.simulation.covariateproviders.samplelevel;
using REpiceaLight.stats.estimates;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            simulatedParameters = new Dictionary<int, Matrix>();
        }

        protected virtual void SetParameterEstimates(E estimate)
        {
            this.parameterEstimates = estimate;
        }

        protected E GetParameterEstimates() { return parameterEstimates; }

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
