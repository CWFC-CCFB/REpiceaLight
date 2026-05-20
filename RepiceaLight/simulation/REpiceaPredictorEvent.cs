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
using System;

namespace REpiceaLight.simulation
{
    public class REpiceaPredictorEvent
    {

        public  class ModelBasedSimulatorEventProperty
        {

            public static readonly ModelBasedSimulatorEventProperty DEFAULT_BETA_JUST_SET = new ModelBasedSimulatorEventProperty("DEFAULT_BETA_JUST_SET");
            public static readonly ModelBasedSimulatorEventProperty DEFAULT_RANDOM_EFFECT_AT_THIS_LEVEL_JUST_SET = new ModelBasedSimulatorEventProperty("DEFAULT_RANDOM_EFFECT_AT_THIS_LEVEL_JUST_SET");
            public static readonly ModelBasedSimulatorEventProperty DEFAULT_RESIDUAL_ERROR_JUST_SET = new ModelBasedSimulatorEventProperty("DEFAULT_RESIDUAL_ERROR_JUST_SET");
            public static readonly ModelBasedSimulatorEventProperty BLUPS_JUST_SET = new ModelBasedSimulatorEventProperty("BLUPS_JUST_SET");
            public static readonly ModelBasedSimulatorEventProperty PARAMETERS_DEVIATE_JUST_GENERATED = new ModelBasedSimulatorEventProperty("PARAMETERS_DEVIATE_JUST_GENERATED");
            public static readonly ModelBasedSimulatorEventProperty RANDOM_EFFECT_DEVIATE_JUST_GENERATED = new ModelBasedSimulatorEventProperty("RANDOM_EFFECT_DEVIATE_JUST_GENERATED");
            public static readonly ModelBasedSimulatorEventProperty RESIDUAL_ERROR_DEVIATE_JUST_GENERATED = new ModelBasedSimulatorEventProperty("RESIDUAL_ERROR_DEVIATE_JUST_GENERATED");

            internal string propertyName;

            protected ModelBasedSimulatorEventProperty(string propertyName)
            {
                this.propertyName = propertyName;
            }

            public string GetPropertyName() { return propertyName; }

            public override string ToString() { return GetPropertyName(); }
        }

        private readonly String propertyName;
        private readonly Object oldValue;
        private readonly Object newValue;
        private readonly REpiceaPredictor source;

        internal REpiceaPredictorEvent(ModelBasedSimulatorEventProperty property, object oldValue, object newValue, REpiceaPredictor source)
        {
            this.propertyName = property.propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.source = source;
        }


        public String GetPropertyName() { return propertyName; }
        public Object GetOldValue() { return oldValue; }
        public Object GetNewValue() { return newValue; }
        public REpiceaPredictor GetSource() { return source; }

    }

}
