using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.math.utility
{

    /// <summary>
    /// A class with useful static mathematic functions.
    /// </summary>
    public class MathUtility
    {

        /// <summary>
        /// Compute the Euclidean distance between two points. <br></br>
        /// This method assumes that the checks have been performed on the coordinates argument.
        /// Basically, these matrices should be column vectors of the same size. 
        /// Each one of them represents a dimension.
        /// </summary>
        /// <param name="i">the index of the first point</param>
        /// <param name="j">the index of the second point</param>
        /// <param name="coordinates">An array of column matrices that stand for the coordinates</param>
        /// <returns>the distance</returns>
        public static double GetEuclideanDistance(int i, int j, Matrix[] coordinates)
        {
            double squareDiffSum = 0d;
            for (int k = 0; k < coordinates.Length; k++)
            {
                Matrix c = coordinates[k];
                double diff = c.GetValueAt(i, 0) - c.GetValueAt(j, 0);
                squareDiffSum += diff * diff;
            }
            return Math.Sqrt(squareDiffSum);
        }


        /// <summary>
        /// Compute the factorial of i. 
        /// </summary>
        /// <param name="i">an integer</param>
        /// <returns>the result as a double</returns>
        /// <exception cref="ArgumentException">If i is smaller than 0</exception>
        public static double Factorial(int i)
        {
            if (i < 0)
                throw new ArgumentException("Parameter i must be equal to or greater than 0!");
            else if (i == 0)
                return 1;
            else
            {
                double result = 1;
                for (int j = 1; j <= i; j++)
                {
                    result *= j;
                }
                return result;
            }
        }

        /// <summary>
        /// Provide the ratio between the factorial of i and the factorial of j.
        /// </summary>
        /// <param name="i">a first integer</param>
        /// <param name="j">a second integer</param>
        /// <returns>the result as a double</returns>
        /// <exception cref="ArgumentException">If i or j are smaller than 0</exception>
        public static double FactorialRatio(int i, int j)
        {
            if (i < 0 || j < 0)
                throw new ArgumentException("Parameters i and j must be equal to or greater than 0!");
            else if (j > i)
                throw new ArgumentException("Parameter j must be smaller than parameter i!");
            else
            {
                if (j == 0)
                    j = 1;
                double result = 1;
                for (int k = i; k > j; k--)
                    result *= k;
                return result;
            }
        }

        ///**
        // * Makes it possible to determine if a double is negative or not. Useful to identify -0.0 for example.
        // * After Peter Lawrey on <a href=https://stackoverflow.com/questions/10399801/how-to-check-if-double-value-is-negative-or-not> StackOverFlow </a>
        // * @param d a real number
        // * @return a boolean true if negative
        // */
        //public static bool IsNegative(double d)
        //{
        //    return Double.doubleToRawLongBits(d) < 0;
        //}

    }
}
