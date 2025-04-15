using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.simulation
{
    public interface IMonteCarloSimulationCompliantObject
    {

        /**
         * This method returns an object that makes it possible to identify
         * the subject that implements this interface. This id remains constant 
         * throughout the Monte Carlo iterations in case of stochastic implementation.
         * @return a String that defines the subject id and that remains constant throughout the simulation
         */
        public String GetSubjectId();

        /**
         * This method returns the hierarchical levels of the object.
         * @return a HierarchicalLevel instance
         */
        public HierarchicalLevel GetHierarchicalLevel();


        //	/**
        //	 * This method sets the MonteCarlo id of the subject. Some instances might have a different implementation of this and might not have to use this method.
        //	 * @param i the MonteCarlo id
        //	 */
        //	public void setMonteCarloRealizationId(int i);


        /**
         * This method returns the id of the Monte Carlo realization. It is necessary for the implementation 
         * of the random deviates on the parameter estimates. These deviates remain constant for a particular
         * Monte Carlo iteration, regardless of the plot.
         * @return an integer
         */
        public int GetMonteCarloRealizationId();

    }

}
