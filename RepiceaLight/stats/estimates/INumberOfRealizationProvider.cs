using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.estimates
{
    public interface INumberOfRealizationsProvider
    {

        /**
         * This method returns the number of realizations the estimate is based on
         * @return an integer
         */
        public int GetNumberOfRealizations();
    }

}
