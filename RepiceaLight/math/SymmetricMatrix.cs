using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.math
{
    public class SymmetricMatrix : Matrix
    {


        public SymmetricMatrix(int size) : base(size, size, true)
        {
        }

        protected override double[][] ContructInternalArray(int iRows, int iCols)
        {
            double[][] mainArray = new double[iRows][];
            for (int i = 0; i < mainArray.Length; i++)
            {
                mainArray[i] = new double[iCols - i];
            }
            return mainArray;
        }

        public override void SetValueAt(int i, int j, double value)
        {
            if (j >= i)
            {
                m_afData[i][j - i] = value;
            }
            else
            {
                m_afData[j][i - j] = value;
            }
        }

        public override double GetValueAt(int i, int j)
        {
            return j >= i ? m_afData[i][j - i] : m_afData[j][i - j];
        }

        public override bool IsSymmetric()
        {
            return true;
        }

        public override Matrix ElementWiseDivide(Matrix m)
        {
            if (IsTheSameDimension(m) && m.IsSymmetric())
            {
                SymmetricMatrix oMat = new SymmetricMatrix(m_iRows);
                for (int i = 0; i < this.m_iRows; i++)
                {
                    for (int j = i; j < this.m_iCols; j++)
                    {
                        oMat.SetValueAt(i, j, GetValueAt(i, j) / m.GetValueAt(i, j));
                    }
                }
                return oMat;
            }
            else
            {
                return base.ElementWiseDivide(m);
            }
        }

        public override bool AnyElementNaN()
        {
            for (int i = 0; i < this.m_iRows; i++)
            {
                for (int j = i; j < this.m_iCols; j++)
                {
                    if (Double.IsNaN(this.GetValueAt(i, j)))
                        return true;
                }
            }
            return false;
        }

        public override Matrix ElementWiseMultiply(Matrix m)
        {
            if (IsTheSameDimension(m) && m.IsSymmetric())
            {
                SymmetricMatrix oMat = new SymmetricMatrix(m_iRows);
                for (int i = 0; i < this.m_iRows; i++)
                {
                    for (int j = i; j < this.m_iCols; j++)
                    {
                        if (GetValueAt(i, j) != 0d && m.GetValueAt(i, j) != 0d)
                        {
                            oMat.SetValueAt(i, j, GetValueAt(i, j) * m.GetValueAt(i, j));
                        }
                    }
                }
                return oMat;
            }
            else
                return base.ElementWiseMultiply(m);
        }

        public override SymmetricMatrix ExpMatrix()
        {
            SymmetricMatrix matrix = new(m_iRows);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = i; j < matrix.m_iCols; j++)
                {
                    matrix.SetValueAt(i, j, Math.Exp(GetValueAt(i, j)));
                }
            }
            return matrix;
        }

        public override SymmetricMatrix LogMatrix()
        {
            bool valid = true;       // default is valid
            SymmetricMatrix matrix = new(m_iRows);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = i; j < matrix.m_iCols; j++)
                {
                    if (GetValueAt(i, j) <= 0d)
                    {
                        valid = false;
                        break;
                    }
                    matrix.SetValueAt(i, j, Math.Log(GetValueAt(i, j)));
                }
                if (!valid)
                    break;
            }
            if (valid)
                return matrix;
            else
                throw new InvalidOperationException("Matrix.LogMatrix() : At least one argument value for the log function is smaller or equal to 0");
        }




        public override Matrix Multiply(Matrix m)
        {
            if (m_iCols != m.m_iRows)
            {
                throw new InvalidOperationException("The matrix m cannot multiply the current matrix for the number of rows is incompatible!");
            }
            else
            {
                if (m.Equals(this))
                {   // multiplied by itself yields a SymmetricMatrix instance
                    SymmetricMatrix mat = new(m_iRows);
                    for (int i_this = 0; i_this < m_iRows; i_this++)
                    {
                        for (int j_m = i_this; j_m < m.m_iCols; j_m++)
                        {
                            for (int j_this = 0; j_this < m_iCols; j_this++)
                            {
                                int i_m = j_this;
                                if (GetValueAt(i_this, j_this) != 0d && m.GetValueAt(i_m, j_m) != 0d)
                                {
                                    double newValue = mat.GetValueAt(i_this, j_m) + GetValueAt(i_this, j_this) * m.GetValueAt(i_m, j_m);
                                    mat.SetValueAt(i_this, j_m, newValue);
                                }
                            }
                        }
                    }
                    return mat;
                }
                else
                    return base.Multiply(m);
            }
        }

        public override SymmetricMatrix ScalarAdd(double d)
        {
            SymmetricMatrix mat = new(m_iRows);
            for (int i = 0; i < m_iRows; i++)
                for (int j = i; j < m_iCols; j++)
                    mat.SetValueAt(i, j, GetValueAt(i, j) + d);
            return mat;
        }

        public override SymmetricMatrix ScalarMultiply(double d)
        {
            SymmetricMatrix mat = new(m_iRows);
            for (int i = 0; i < m_iRows; i++)
                for (int j = i; j < m_iCols; j++)
                    mat.SetValueAt(i, j, GetValueAt(i, j) * d);
            return mat;
        }

        public override Matrix Add(Matrix m)
        {
            if (!IsTheSameDimension(m))
                throw new InvalidOperationException("This instance and the Matrix m are not of the same dimension!");
            if (m.IsSymmetric())
            {
                SymmetricMatrix mat = new SymmetricMatrix(m_iRows);
                for (int i = 0; i < m_iRows; i++)
                    for (int j = i; j < m_iCols; j++)
                        mat.SetValueAt(i, j, GetValueAt(i, j) + m.GetValueAt(i, j));
                return mat;
            }
            else
                return base.Add(m);
        }


        public override Matrix Subtract(Matrix m)
        {
            if (!IsTheSameDimension(m))
                throw new InvalidOperationException("This instance and the Matrix m are not of the same dimension!");
            if (m.IsSymmetric())
            {
                SymmetricMatrix mat = new(m_iRows);
                for (int i = 0; i < m_iRows; i++)
                    for (int j = i; j < m_iCols; j++)
                        mat.SetValueAt(i, j, GetValueAt(i, j) - m.GetValueAt(i, j));
                return mat;
            }
            else
                return base.Subtract(m);
        }

        /// <summary>
        /// Create a vector of the values corresponding to a symmetric matrix.
        /// </summary>
        /// <returns>a column vector</returns>
        public Matrix SymSquare()
        {
            int numberOfElements = (m_iCols + 1) * m_iCols / 2;
            Matrix outputMatrix = new(numberOfElements, 1);
            int pointer = 0;
            Matrix tmp;
            for (int i = 0; i < m_iCols; i++)
            {
                tmp = GetSubMatrix(i, i, 0, i).Transpose();     // transpose required to get a column vector
                outputMatrix.SetSubMatrix(tmp, pointer, 0);
                pointer += tmp.GetNumberOfElements();
            }
            return outputMatrix;
        }

        public override Matrix Transpose()
        {
            return this;
        }

        public override SymmetricMatrix PowMatrix(double seed)
        {
            SymmetricMatrix matrix = new(m_iRows);
            for (int i = 0; i < matrix.m_iRows; i++)
                for (int j = i; j < matrix.m_iCols; j++)
                    matrix.SetValueAt(i, j, Math.Pow(seed, GetValueAt(i, j)));
            return matrix;
        }

        public override SymmetricMatrix ElementWisePower(double power)
        {
            SymmetricMatrix matrix = new SymmetricMatrix(m_iRows);
            for (int i = 0; i < matrix.m_iRows; i++)
                for (int j = i; j < matrix.m_iCols; j++)
                    matrix.SetValueAt(i, j, Math.Pow(GetValueAt(i, j), power));
            return matrix;
        }


        public override SymmetricMatrix GetAbsoluteValue()
        {
            SymmetricMatrix oMat = new(m_iRows);
            for (int i = 0; i < m_iRows; i++)
                for (int j = i; j < m_iCols; j++)
                    oMat.SetValueAt(i, j, Math.Abs(GetValueAt(i, j)));
            return oMat;
        }


        public override Matrix GetKroneckerProduct(Matrix m)
        {
            if (m.IsSymmetric())
            {
                SymmetricMatrix result = new(m_iRows * m.m_iRows);
                for (int i1 = 0; i1 < m_iRows; i1++)
                    for (int j1 = i1; j1 < m_iCols; j1++)
                        for (int i2 = 0; i2 < m.m_iRows; i2++)
                            for (int j2 = 0; j2 < m.m_iCols; j2++)
                                result.SetValueAt(i1 * m.m_iRows + i2, j1 * m.m_iCols + j2, GetValueAt(i1, j1) * m.GetValueAt(i2, j2));
                return result;
            }
            else
                return base.GetKroneckerProduct(m);
        }

        /**
         * Create a Matrix that corresponds to the Isserlis theorem given that matrix this is
         * a variance-covariance matrix.
         * @return a SymmetricMatrix instance
         */
        public SymmetricMatrix GetIsserlisMatrix()
        {
            SymmetricMatrix output = new(m_iRows * m_iRows);
            double covariance;
            int indexRow;
            int indexCol;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = i; j < m_iCols; j++)
                {
                    for (int iPrime = 0; iPrime < m_iRows; iPrime++)
                    {
                        for (int jPrime = 0; jPrime < m_iCols; jPrime++)
                        {
                            covariance = GetValueAt(i, j) * GetValueAt(iPrime, jPrime) +
                                    GetValueAt(i, iPrime) * GetValueAt(j, jPrime) +
                                    GetValueAt(i, jPrime) * GetValueAt(j, iPrime);
                            indexRow = i * m_iRows + iPrime;
                            indexCol = j * m_iCols + jPrime;
                            output.SetValueAt(indexRow, indexCol, covariance);
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Compute the lower triangle of the Cholesky decomposition.<br></br>
        ///  Checks are implemented to make sure that this is square and symmetric.
        /// </summary>
        /// <returns>the resulting matrix </returns>
        /// <exception cref="InvalidOperationException">if the Cholesky factorisation cannot be completed</exception>
        public virtual Matrix GetLowerCholTriangle()
        {
            int m1Row = m_iRows;
            Matrix matrix = new(m1Row, m1Row);
            double dTmp;
            for (int i = 0; i < m1Row; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (j == i)
                    {
                        dTmp = 0;
                        for (int k = 0; k <= i - 1; k++)
                        {
                            dTmp += matrix.GetValueAt(i, k) * matrix.GetValueAt(i, k);
                        }
                        matrix.SetValueAt(i, j, Math.Sqrt(GetValueAt(i, j) - dTmp));
                    }
                    else
                    {
                        dTmp = 0;
                        for (int k = 0; k <= j - 1; k++)
                        {
                            dTmp += matrix.GetValueAt(i, k) * matrix.GetValueAt(j, k);
                        }
                        matrix.SetValueAt(i, j, 1d / matrix.GetValueAt(j, j) * (GetValueAt(i, j) - dTmp));
                    }
                    if (Double.IsNaN(matrix.GetValueAt(i, j)))
                    {
                        throw new InvalidOperationException("Matrix.LowerChol(): the lower triangle of the Cholesky decomposition cannot be calculated because NaN have been generated!");
                    }
                }
            }
            return matrix;
        }


        /**
         * Check if the matrix is positive definite. The check is based on the Cholesky factorization. If the factorization can
         * be computed the method returns true.
         * @return true if it is or false otherwise
         */
        public bool IsPositiveDefinite()
        {
            try
            {
                GetLowerCholTriangle();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public override SymmetricMatrix Clone()
        {
            SymmetricMatrix oMat = new(m_iRows);
            for (int i = 0; i < m_iRows; i++)
                for (int j = i; j < m_iCols; j++)
                    oMat.SetValueAt(i, j, GetValueAt(i, j));
            return oMat;
        }

        internal override SymmetricMatrix GetInternalInverseMatrix()
        {
            Matrix m = base.GetInternalInverseMatrix();
            return ForceConversionToSymmetricMatrix(m);
        }

        /**
         * Try to convert a Matrix instance to a SymmetricMatrix instance.
         * @param m a Matrix instance
         * @return a SymmetricMatrix instance
         */
        public static SymmetricMatrix ConvertToSymmetricIfPossible(Matrix m)
        {
            if (m is SymmetricMatrix)
                return (SymmetricMatrix)m;
            else if (!m.IsSymmetric())
                throw new InvalidOperationException("The Matrix instance m is not symmetric!");
            else
                return ForceConversionToSymmetricMatrix(m);
        }

        static SymmetricMatrix ForceConversionToSymmetricMatrix(Matrix m)
        {
            if (m is SymmetricMatrix)
                return (SymmetricMatrix)m;
            else
            {
                if (!m.IsSquare())
                    throw new InvalidOperationException("Matrix m must be a square matrix!");
                else
                {
                    SymmetricMatrix sm = new(m.m_iRows);
                    for (int i = 0; i < m.m_iRows; i++)
                        for (int j = i; j < m.m_iRows; j++)
                            sm.SetValueAt(i, j, m.GetValueAt(i, j));
                    return sm;
                }
            }
        }


    public override Matrix MatrixDiagBlock(Matrix m)
        {
            if (m.IsSymmetric())
            {
                int m1Row = m_iRows;
                int m2Row = m.m_iRows;
                SymmetricMatrix matrix = new(m1Row + m2Row);
                for (int i = 0; i < m1Row; i++)
                    for (int j = i; j < m1Row; j++)
                        matrix.SetValueAt(i, j, GetValueAt(i, j));
                for (int i = 0; i < m2Row; i++)
                    for (int j = i; j < m2Row; j++)
                        matrix.SetValueAt(i + m1Row, j + m1Row, m.GetValueAt(i, j));
                return matrix;
            }
            else
                return base.MatrixDiagBlock(m);
        }


        public override SymmetricMatrix GetInverseMatrix()
        {
            if (IsDiagonalMatrix())
            {       // procedure for diagonal matrices
                return DiagonalMatrix.ForceConversionToDiagonalMatrix(this).GetInverseMatrix();
            }
            List<List<int>> indices = GetBlockConfiguration();
            if (indices.Count == 1)
                return GetInternalInverseMatrix();
            else
            {
                SymmetricMatrix inverseMatrix = new(m_iRows);
                foreach (List<int> blockIndex in indices)
                {
                    Matrix invSubMatrix = GetSubMatrix(blockIndex, blockIndex).GetInternalInverseMatrix();
                    for (int i = 0; i < blockIndex.Count; i++)
                        for (int j = i; j < blockIndex.Count; j++)
                            inverseMatrix.SetValueAt(blockIndex[i], blockIndex[j], invSubMatrix.GetValueAt(i, j));
                }
                return inverseMatrix;
            }
        }

    }
}
