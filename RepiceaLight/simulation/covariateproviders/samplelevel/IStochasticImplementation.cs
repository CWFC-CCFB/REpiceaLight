using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.simulation.covariateproviders.samplelevel
{
    public interface IStochasticImplementation
    {

        /**
         * Return true if the instance is running in stochastic mode or false it is in deterministic mode.
         * @return a boolean
         */
        public bool IsStochastic();

    }
}
