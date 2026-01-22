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
    public interface IHeightMProvider
    {
        /**
	     * This method returns the tree height in m.
	     * @return the height in m (double)
	     */
        public double GetHeightM();


    }
}
