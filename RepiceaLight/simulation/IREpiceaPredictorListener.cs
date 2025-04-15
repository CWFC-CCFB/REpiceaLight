using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.simulation
{
    public interface IREpiceaPredictorListener
    {
        public void ModelBasedSimulatorDidThis(REpiceaPredictorEvent ev);

    }
}
