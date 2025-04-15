using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.IDistribution;
using static REpiceaLight.stats.StatisticalUtility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public sealed class CenteredGaussianDistribution : IDistribution
    {


        private readonly GaussianDistribution underlyingDistribution;
        private readonly double correlationParameter;
        private readonly TypeMatrixR? matrixType;
        private readonly bool isStructured;

        private readonly Dictionary<List<int>, SymmetricMatrix> structuredVarianceCovarianceMap;
        private readonly Dictionary<List<int>, Matrix> structuredLowerCholeskyMap;
        private readonly Dictionary<int, SymmetricMatrix> simpleVarianceCovarianceMap;
        private readonly Dictionary<int, Matrix> simpleLowerCholeskyMap;

        /**
         * General constructor.
         * @param variance the homogeneous variance
         * @param correlationParameter the correlation parameter in the correlation structure
         * @param type a TypeMatrixR enum
         */
        public CenteredGaussianDistribution(SymmetricMatrix variance, double correlationParameter, TypeMatrixR? matrixType)
        {
            underlyingDistribution = new GaussianDistribution(new Matrix(variance.m_iRows, 1), variance);
            this.correlationParameter = correlationParameter;
            this.matrixType = matrixType;
            isStructured = this.correlationParameter != 0 && this.matrixType != null;
            if (isStructured && variance.m_iRows > 1)
                throw new ArgumentException("The CenteredGaussianDistribution is not designed for a multivariate distribution with heterogeneous variances yet.");
            structuredVarianceCovarianceMap = new();
            structuredLowerCholeskyMap = new();
            simpleVarianceCovarianceMap = new();
            simpleLowerCholeskyMap = new();
        }

        /**
         * Constructor for univariate distribution.
         * @param variance the homogeneous variance
         */
          public CenteredGaussianDistribution(SymmetricMatrix variance) : this(variance, 0d, null)
          {

         }

        private Matrix GetLowerCholesky(List<int> indexList)
        {
            if (IsStructured())
            {
                if (!structuredLowerCholeskyMap.ContainsKey(indexList))
                    UpdateMaps(indexList);
                return structuredLowerCholeskyMap[indexList];
            }
            else
            {
                int size = indexList.Count;
                if (!simpleLowerCholeskyMap.ContainsKey(size))
                    UpdateMaps(size);

                return simpleLowerCholeskyMap[size];
            }
        }

        private void UpdateMaps(object key)
        {
            if (key is List<int>)
            {
                List<int> referenceList = new();
                referenceList.AddRange((List<int>)key);      // make a copy to avoid changes through reference
                Matrix distances = new(referenceList);
                if (!matrixType.HasValue)
                    throw new InvalidOperationException("The R Matrix has not been set!");
                SymmetricMatrix correlationMatrix = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, correlationParameter }, matrixType.Value, distances);
                SymmetricMatrix varianceCovariance = correlationMatrix.ScalarMultiply(underlyingDistribution.GetVariance().GetValueAt(0, 0));
                structuredVarianceCovarianceMap[referenceList] = varianceCovariance;
                Matrix lowerChol = varianceCovariance.GetLowerCholTriangle();
                structuredLowerCholeskyMap[referenceList] = lowerChol;
            }
            else
            {
                int size = (int)key;
                DiagonalMatrix varianceCovariance = Matrix.GetIdentityMatrix(size).ScalarMultiply(underlyingDistribution.GetVariance().GetValueAt(0, 0));
                simpleVarianceCovarianceMap[size] = varianceCovariance;
                Matrix lowerChol = varianceCovariance.GetLowerCholTriangle();
                simpleLowerCholeskyMap[size] = lowerChol;
            }
        }

        private SymmetricMatrix GetVariance(List<int> indexList)
        {
            if (IsStructured())
            {
                if (!structuredVarianceCovarianceMap.ContainsKey(indexList))
                    UpdateMaps(indexList);

                return structuredVarianceCovarianceMap[indexList];
            }
            else
            {
                int size = indexList.Count;
                if (!simpleVarianceCovarianceMap.ContainsKey(size))
                    UpdateMaps(size);

                return simpleVarianceCovarianceMap[size];
            }
        }

        public Matrix GetMean()
        {
            return underlyingDistribution.GetMean();
        }

        /**
         * This method should be used in preference to the getMean() method.
         * @param errorTermList a GaussianErrorTermList instance
         * @return a Matrix instance
         */
        public Matrix GetMean(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
            Matrix chol = GetLowerCholesky(errorTermList.GetDistanceIndex());
            return chol.Multiply(errorTermList.GetNormalizedErrors());
        }

        public SymmetricMatrix GetVariance()
        {
            return underlyingDistribution.GetVariance();
        }

        /**
         * Provide the variance of the distribution given some error terms.<p>
         * The class adapts the variance matrix as the number of error terms increases.
         * @param errorTermList a GaussianErrorTermList instance
         * @return a SymmetricMatrix instance
         */
        public SymmetricMatrix GetVariance(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
  
            return GetVariance(errorTermList.GetDistanceIndex());
        }

        public Matrix GetRandomRealization()
        {
            return underlyingDistribution.GetRandomRealization();
        }

        public Matrix GetRandomRealization(GaussianErrorTermList errorTermList)
        {
            if (errorTermList == null || errorTermList.Count == 0)
                throw new ArgumentException("The errorTermList argument should be a non empty GaussianErrorTermList instance!");
            Matrix errorTerms;
            if (!errorTermList.updated)
            {
                List<int> indexList = errorTermList.GetDistanceIndex();
                Matrix stdMat = GetLowerCholesky(indexList);
                Matrix normalizedErrorTerms = errorTermList.GetNormalizedErrors();
                errorTerms = stdMat.Multiply(normalizedErrorTerms);
                errorTermList.UpdateErrorTerm(errorTerms);
            }
            else
            {
                errorTerms = errorTermList.GetRealizedErrors();
            }
            return errorTerms;
        }
 


        public bool IsParametric() { return true; }

        public bool IsMultivariate() { return GetMean().m_iRows > 1; }

        public DistributionType GetDistributionType() { return DistributionType.GAUSSIAN; }

        public bool IsStructured() { return isStructured; }

        //	public static void main(String[] args) {
        //		List<Integer> list1 = new ArrayList<Integer>();
        //		list1.add(1);
        //		list1.add(2);
        //		List<Integer> list2 = new ArrayList<Integer>();
        //		list2.add(2);
        //		list2.add(1);
        //		System.out.println("Lists are equal : " + list1.equals(list2));
        //		Collections.sort(list2);
        //		System.out.println("Lists are equal : " + list1.equals(list2));
        //	
        //	}

    }

}
