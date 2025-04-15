using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal REpiceaPredictorEvent(ModelBasedSimulatorEventProperty property, object? oldValue, object newValue, REpiceaPredictor source)
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
