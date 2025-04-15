using REpiceaLight.math;
using REpiceaLight.math.utility;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.StatisticalUtility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats
{
    public sealed class StatisticalUtility
    {

        private static REpiceaRandom random;

        public enum TypeMatrixR
        {
            LINEAR,
            LINEAR_LOG,
            COMPOUND_SYMMETRY,
            POWER,
            ARMA,
            EXPONENTIAL
        }

        private static readonly Dictionary<TypeMatrixR, int> NbParmsMap = new();

        static StatisticalUtility()
        {
            NbParmsMap[TypeMatrixR.LINEAR] = 2;
            NbParmsMap[TypeMatrixR.LINEAR_LOG] = 2;
            NbParmsMap[TypeMatrixR.COMPOUND_SYMMETRY] = 2;
            NbParmsMap[TypeMatrixR.POWER] = 2;
            NbParmsMap[TypeMatrixR.ARMA] = 3;
            NbParmsMap[TypeMatrixR.EXPONENTIAL] = 2;
        }




        /**
         * This method is a shortcut for inverting an AR1 correlation matrix.
         * @param size the size of the matrix
         * @param rho the correlation between two successive observations
         * @return a Matrix
         */
        public static Matrix GetInverseCorrelationAR1Matrix(int size, double rho)
        {
            if (size < 1)
                throw new ArgumentException("The size parameter must be equal to or greater than 1!");
            if (rho <= 0 || rho >= 1)
                throw new ArgumentException("The rho parameter must be greater than 0 and smaller than 1!");
            double rho2 = rho * rho;
            Matrix mat = new(size, size);
            for (int i = 0; i < mat.m_iRows; i++)
            {
                for (int j = i; j < mat.m_iCols; j++)
                {
                    if (j == i)
                    {
                        if (i == 0 || i == mat.m_iRows - 1)
                            mat.SetValueAt(i, j, 1d / (1d - rho2));
                        else
                            mat.SetValueAt(i, j, (1d + rho2) / (1d - rho2));
                    }
                    else if (j == i + 1)
                    {
                        mat.SetValueAt(i, j, -rho / (1d - rho2));
                        mat.SetValueAt(j, i, -rho / (1d - rho2));
                    }
                }
            }
            return mat;
        }

        //  /**
        //* Construct a within-subject correlation matrix using a variance parameter, a correlation parameter and a column vector of coordinates. <p>
        //* @param coordinates a column vector of coordinates from which the distances are calculated
        //* @param varianceParameter the variance parameter
        //* @param covarianceParameter the covariance parameter
        //* @param type the type of correlation
        //* @return the resulting matrix
        //* @deprecated This method is no longer acceptable. Use {@link #constructRMatrix(List, TypeMatrixR, Matrix...)} instead.
        //*/
        //  @Deprecated
        //  protected static Matrix constructRMatrix(Matrix coordinates, double varianceParameter, double covarianceParameter, TypeMatrixR type)
        //  {
        //      if (!coordinates.isColumnVector())
        //      {
        //          throw new UnsupportedOperationException("Matrix.constructRMatrix() : The coordinates matrix is not a column vector");
        //      }
        //      else
        //      {
        //          int nrow = coordinates.m_iRows;
        //          Matrix matrixR = new Matrix(nrow, nrow);
        //          for (int i = 0; i < nrow; i++)
        //          {
        //              for (int j = i; j < nrow; j++)
        //              {
        //                  double corr = 0d;
        //                  switch (type)
        //                  {
        //                      case LINEAR:                    // linear case
        //                          corr = 1 - covarianceParameter * Math.abs(coordinates.getValueAt(i, 0) - coordinates.getValueAt(j, 0));
        //                          if (corr >= 0)
        //                          {
        //                              matrixR.setValueAt(i, j, varianceParameter * corr);
        //                              matrixR.setValueAt(j, i, varianceParameter * corr);
        //                          }
        //                          break;
        //                      case LINEAR_LOG:                // linear log case
        //                          if (Math.abs(coordinates.getValueAt(i, 0) - coordinates.getValueAt(j, 0)) == 0)
        //                          {
        //                              corr = 1d;
        //                          }
        //                          else
        //                          {
        //                              corr = 1 - covarianceParameter * Math.log(Math.abs(coordinates.getValueAt(i, 0) - coordinates.getValueAt(j, 0)));
        //                          }
        //                          if (corr >= 0)
        //                          {
        //                              matrixR.setValueAt(i, j, varianceParameter * corr);
        //                              matrixR.setValueAt(j, i, varianceParameter * corr);
        //                          }
        //                          break;
        //                      case COMPOUND_SYMMETRY:
        //                          if (i == j)
        //                          {
        //                              matrixR.setValueAt(i, j, varianceParameter + covarianceParameter);
        //                          }
        //                          else
        //                          {
        //                              matrixR.setValueAt(i, j, covarianceParameter);
        //                              matrixR.setValueAt(j, i, covarianceParameter);
        //                          }
        //                          break;
        //                      case POWER:                  // power case
        //                          if (Math.abs(coordinates.getValueAt(i, 0) - coordinates.getValueAt(j, 0)) == 0)
        //                          {
        //                              corr = 1d;
        //                          }
        //                          else
        //                          {
        //                              corr = Math.pow(covarianceParameter, (Math.abs(coordinates.getValueAt(i, 0) - coordinates.getValueAt(j, 0))));
        //                          }
        //                          if (corr >= 0)
        //                          {
        //                              matrixR.setValueAt(i, j, varianceParameter * corr);
        //                              matrixR.setValueAt(j, i, varianceParameter * corr);
        //                          }
        //                          break;
        //                      default:
        //                          throw new UnsupportedOperationException("Matrix.ConstructRMatrix() : This type of correlation structure: " + type + " is not supported in this function");
        //                  }
        //              }
        //          }
        //          return matrixR;
        //      }
        //  }

        public static SymmetricMatrix ConstructRMatrix(List<double> covParms, TypeMatrixR type, Matrix coordinates)
        {
            return ConstructRMatrix(covParms, type, [coordinates]);
        }


        /**
         * Compute the R matrix. <p>
         * Compute the R matrix of the type set by the type argument. 
         * 
         * @param covParms a List of double containing the parameter. The first is the variance parameter, the second is the 
         * covariance parameter. In case of ARMA type, there is a third parameter which is the gamma parameter.
         * @param type a TypeMatrixR enum
         * @param coordinates a series of Matrices instance that stand for the coordinates. These should be column vectors of 
         * the same size. Specifying two matrices implies that the Euclidean distance is based on two dimensions. Three matrices
         * means three dimensions and so on.
         * @return a SymmetricMatrix instance
         */
        public static SymmetricMatrix ConstructRMatrix(List<double> covParms, TypeMatrixR type, Matrix[] coordinates)
        {
            if (covParms == null || covParms.Count < NbParmsMap[type])
                throw new ArgumentException("The covParms list should contain this number of parameters: " + NbParmsMap[type] + " when using type " + Enum.GetName(type));
            if (coordinates == null || coordinates.Length == 0)
                throw new ArgumentException("The coordinates argument should contain at least one matrix.");
            int nrow = -1;
            // check if the coordinates argument complies
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (!coordinates[i].IsColumnVector())
                    throw new ArgumentException("The coordinates should contain only column vectors!");
                else
                {
                    if (nrow == -1)
                        nrow = coordinates[i].m_iRows;
                    else if (coordinates[i].m_iRows != nrow)
                        throw new ArgumentException("The coordinates should contain only column vectors of the same size!");
                }
            }

            double varianceParameter = covParms[0];
            double covarianceParameter = covParms[1];
            double gamma = type == TypeMatrixR.ARMA ? covParms[2] : 0;

            double distance;
            SymmetricMatrix matrixR = new SymmetricMatrix(nrow);
            for (int i = 0; i < nrow; i++)
            {
                for (int j = i; j < nrow; j++)
                {
                    double corr = 0d;
                    switch (type)
                    {
                        case TypeMatrixR.LINEAR:                    // linear case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            corr = 1 - covarianceParameter * distance;
                            if (corr >= 0)
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            break;
                        case TypeMatrixR.LINEAR_LOG:                // linear log case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            corr = distance == 0 ? 1d : 1 - covarianceParameter * Math.Log(distance);
                            if (corr >= 0)
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            break;
                        case TypeMatrixR.COMPOUND_SYMMETRY:
                            if (i == j)
                                matrixR.SetValueAt(i, j, varianceParameter + covarianceParameter);
                            else
                                matrixR.SetValueAt(i, j, covarianceParameter);
                            break;
                        case TypeMatrixR.POWER:                  // power case
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            if (distance == 0)
                            {
                                corr = 1d;
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            else
                            {
                                corr = Math.Pow(covarianceParameter, distance);
                                if (corr >= 0)
                                    matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            break;
                        case TypeMatrixR.ARMA:
                            if (i == j)
                                matrixR.SetValueAt(i, i, varianceParameter);
                            else
                            {
                                distance = Math.Abs(i - j) - 1;
                                double powCol = Math.Pow(covarianceParameter, distance);
                                matrixR.SetValueAt(i, j, varianceParameter * gamma * powCol);
                            }
                            break;
                        case TypeMatrixR.EXPONENTIAL:
                            distance = MathUtility.GetEuclideanDistance(i, j, coordinates);
                            if (distance == 0)
                            {
                                corr = 1d;
                                matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            else
                            {
                                corr = Math.Exp(-distance / covarianceParameter);
                                if (corr >= 0)
                                    matrixR.SetValueAt(i, j, varianceParameter * corr);
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Matrix.ConstructRMatrix() : This type of correlation structure is not supported in this function");
                    }
                }
            }
            return matrixR;
        }



        /**
         * Generate a random vector
         * @param nrow the number of elements to be generated
         * @param type the distribution type (a Distribution.Type enum variable)
         * @return a Matrix instance
         */
        public static Matrix DrawRandomVector(int nrow, IDistribution.DistributionType type)
        {
            return StatisticalUtility.DrawRandomVector(nrow, type, StatisticalUtility.GetRandom());
        }


        /**
         * Return a Random generator.
         * @return a Random instance
         */
        public static REpiceaRandom GetRandom()
        {
            random ??= new REpiceaRandom();
            return random;
        }

        /**
         * This method generates a random vector
         * @param nrow the number of elements to be generated
         * @param type the distribution type (a Distribution enum variable)
         * @param random a Random instance
         * @return a Matrix instance
         */
        public static Matrix DrawRandomVector(int nrow, IDistribution.DistributionType type, REpiceaRandom random)
        {
            //try
            //{
            //            bool valid = true;
            Matrix matrix = new(nrow, 1);
            for (int i = 0; i < nrow; i++)
            {
                double number;
                switch (type)
                {
                    case IDistribution.DistributionType.GAUSSIAN:      // Gaussian random number ~ N(0,1)
                        number = random.NextGaussian();
                        break;
                    case IDistribution.DistributionType.UNIFORM:       // Uniform random number [0,1]
                        number = random.NextDouble();
                        break;
                    default:
                        throw new InvalidOperationException("Matrix.RandomVector() : The specified distribution is not supported in the function");
                        //i = nrow;
                        //Console.WriteLine();
                        //valid = false;
                        //break;
                }
                //                if (valid)
                matrix.SetValueAt(i, 0, number);
            }
            //          if (valid)
            return matrix;
            //else
            //    return null;
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Matrix.RandomVector() : Error while computing the random vector");
            //    return null;
            //}
        }

        /**
         * This method returns the number of combinations.
         * @param n the number of units
         * @param d the number of units drawn in each combination
         * @return a long
         */
        public static long GetCombinations(int n, int d)
        {
            if (n < 1 || d < 1)
                throw new ArgumentException("Parameters n and d must be equal to or greater than 1!");
            else if (d > n)
                throw new ArgumentException("Parameters d must be equal to or smaller than parameter n!");
            double r = n - d > d ?
                 MathUtility.FactorialRatio(n, n - d) / MathUtility.Factorial(d) :
                    MathUtility.FactorialRatio(n, d) / MathUtility.Factorial(n - d);
            return (long)r; // TODO check if this cast works
        }





    }
}
