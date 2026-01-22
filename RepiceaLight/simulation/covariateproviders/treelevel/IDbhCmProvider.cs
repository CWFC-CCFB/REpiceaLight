using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.simulation.covariateproviders.treelevel
{
    /**
     * This interface ensures the tree instance can provide its own dbh.
     * @author Mathieu Fortin - November 2012
     */
    public interface IDbhCmProvider
    {
        /**
         * This method returns the diameter at breast height of the tree.
         * @return the dbh in cm (double)
         */
        public double GetDbhCm();

    }
}
