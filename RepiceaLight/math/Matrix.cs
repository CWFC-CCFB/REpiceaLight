using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace REpiceaLight.math
{
    /**
     * This class implement most of the basic function in linear algebra
     * Authors: Jean-Francois Lavoie and Mathieu Fortin (June 2009)
     */
    public class Matrix : AbstractMatrix<Matrix>
    {

        protected static int SizeBeforeSwitchingToLUDecompositionInDeterminantCalculation = 6;

        public static int NB_ROWS_BEYOND_WHICH_MATRIX_INVERSION_TAKES_TOO_MUCH_TIME = 600;


        //   private static readonly NumberFormat SimpleDecimalFormatter = NumberFormat.getNumberInstance();
        //static {
        //       SimpleDecimalFormatter.setMinimumFractionDigits(2);
        //   }

        private static readonly double VERY_SMALL = 1.5E-06;
        private static readonly double EPSILON = 1E-12;

        protected internal readonly double[][] m_afData;

        /**
         * Constructor 1. Creates a matrix from a two-dimension array.
         * @param data a two-dimension array of double
         */
        public Matrix(double[][] data) : this(data.Length, data[0].Length, true)
        {
            for (int i = 0; i < m_iRows; i++)
                for (int j = 0; j < m_iCols; j++)
                    SetValueAt(i, j, data[i][j]);
        }

        /**
         * Constructor 2. Creates a column vector from an array of double
         * @param data an array of double instances.
         */
        public Matrix(double[] data) : this(data.Length, 1, true)
        {
            for (int i = 0; i < m_iRows; i++)
                SetValueAt(i, 0, data[i]);
        }

        /**
         * Constructor 3. Creates a column vector with all the values found in the List instance.
         * @param list a List of Number-derived instances
         */
        public Matrix(List<double> list) : this(list.Count, 1, true)
        {
            double d;
            for (int i = 0; i < m_iRows; i++)
            {
                d = list[i];
                SetValueAt(i, 0, d);
            }
        }

        public Matrix(List<int> list) : this(list.Count, 1, true)
        {
            double d;
            for (int i = 0; i < m_iRows; i++)
            {
                d = list[i];
                SetValueAt(i, 0, d);
            }
        }

        /**
         * Constructor 4. Creates a matrix with the elements starting from a given number with a particular increment.
         * @param iRows number of rows
         * @param iCols number of columns
         * @param from first element of the matrix
         * @param iIncrement increment for the next elements
         */
        public Matrix(int iRows, int iCols, double from, double iIncrement) : this(iRows, iCols, true)
        {

            double value = from;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    SetValueAt(i, j, value);
                    value += iIncrement;
                }
            }
        }

        public Matrix(int iRows, int iCols) : this(iRows, iCols, true) { }


        /**
         * Protected constructor which allows for the former implementation. 
         * @param iRows number of rows
         * @param iCols number of columns
         * @param newImplementation if true, the column vector are stored in transposed manner so that they take less memory. 
         * By default, it is set to true.
         */
        protected internal Matrix(int iRows, int iCols, bool newImplementation) : base(iRows, iCols)
        {
            if (iCols == 1 && newImplementation)
            {
                m_afData = ContructInternalArray(iCols, iRows);     // the array is stored as a row vector for better memory management
            }
            else
            {
                m_afData = ContructInternalArray(iRows, iCols);
            }
        }

        protected virtual double[][] ContructInternalArray(int iRows, int iCols)
        {
            double[][] myArray = new double[iRows][];
            for (int j = 0; j < myArray.Length; j++)
            {
                myArray[j] = new double[iCols];
            }

            return myArray;
        }

        /**
         * Create an array from the Matrix instance. Note that this array is not the internal array. It is a copy and consequently repeated calls to 
         * this method with large matrices might be computationally intensive.
         * @return a 2-dimension array
         */
        public double[][] ToArray()
        {
            double[][] arr = new double[m_iRows][];
            for (int i = 0; i < m_iRows; i++)
            {
                arr[i] = new double[m_iCols];
                for (int j = 0; j < m_iCols; j++)
                {
                    arr[i][j] = GetValueAt(i, j);
                }
            }
            return arr;
        }

        private bool IsNewImplementationForColumnVector()
        {
            return IsColumnVector() && !IsRowVector() && m_afData.Length == 1;  // second condition is to avoid side effect when dealing with a 1x1 matrix
        }


        /// <summary>
        /// Set the value at row i and column j.
        /// </summary>
        /// <param name="i">the row index</param>
        /// <param name="j">the column index</param>
        /// <param name="value">the value to be set in the cell</param>
        public virtual void SetValueAt(int i, int j, double value)
        {
            if (IsNewImplementationForColumnVector())
            {   // the vector is actually transposed for a better memory management
                m_afData[j][i] = value;
            }
            else
            {
                m_afData[i][j] = value;
            }
        }


        /// <summary>
        /// Return the value at row i and column j 
        /// </summary>
        /// <param name="i">the row index</param>
        /// <param name="j">the column index</param>
        /// <returns>the entry</returns>
        public virtual double GetValueAt(int i, int j)
        {
            return IsNewImplementationForColumnVector() ? m_afData[j][i] : m_afData[i][j];
        }

        public override Matrix Add(Matrix m)
        {
            if (!IsTheSameDimension(m))
            {
                throw new InvalidOperationException("This instance and the Matrix m are not of the same dimension!");
            }
            Matrix mat = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    mat.SetValueAt(i, j, GetValueAt(i, j) + m.GetValueAt(i, j));
                }
            }

            return mat;
        }

        /**
         * This method tests whether if any element of the Matrix object is 
         * different from parameter d. 
         * @param d the value to be checked
         * @return true if at least one element is different from d
         */
        public bool AnyElementDifferentFrom(double d)
        {
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (Math.Abs(GetValueAt(i, j) - d) > EPSILON)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * This method tests whether if any element of the Matrix object is 
         * larger than parameter d. 
         * @param d the value to be checked
         * @return true if at least one element is larger than d
         */
        public bool AnyElementLargerThan(double d)
        {
            for (int i = 0; i < this.m_iRows; i++)
            {
                for (int j = 0; j < this.m_iCols; j++)
                {
                    if (GetValueAt(i, j) > d)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /**
         * This method tests whether if any element of the Matrix object is 
         * smaller than or equal to parameter d. 
         * @param d the value to be checked
         * @return true if at least one element is larger than d
         */
        public bool AnyElementSmallerOrEqualTo(double d)
        {
            for (int i = 0; i < this.m_iRows; i++)
            {
                for (int j = 0; j < this.m_iCols; j++)
                {
                    if (GetValueAt(i, j) <= d)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any element is a "Not a Number" (NaN)
        /// </summary>
        /// <returns>true if at least one element is NaN</returns>
        public virtual bool AnyElementNaN()
        {
            for (int i = 0; i < this.m_iRows; i++)
            {
                for (int j = 0; j < this.m_iCols; j++)
                {
                    if (Double.IsNaN(this.GetValueAt(i, j)))
                        return true;
                }
            }
            return false;
        }


        /**
         * This method return a vector that contains the diagonal element of this Matrix instance.
         * A check is implemented to make sure this is a square matrix.
         * @return the resulting matrix
         * @throws UnsupportedOperationException if the matrix is not square
         */
        public Matrix DiagonalVector()
        {
            if (!IsSquare())
            {
                throw new InvalidOperationException("Matrix.diagonalVector() : The input matrix is not square");
            }
            else
            {
                Matrix oMat = new Matrix(m_iRows, 1);
                for (int i = 0; i < m_iRows; i++)
                {
                    oMat.SetValueAt(i, 0, GetValueAt(i, i));
                }
                return oMat;
            }
        }

        /**
         * This method returns true if the matrix contains at least one value NaN.
         * @return a boolean
         */
        public bool DoesContainAnyNaN()
        {
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (Double.IsNaN(GetValueAt(i, j)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Compute the elementwise division of this by m
        /// </summary>
        /// <param name="m">a Matrix instance of the same dimensions</param>
        /// <returns>a Matrix</returns>
        /// <exception cref="InvalidOperationException">if matrix m does not have the same dimensions</exception>
        public virtual Matrix ElementWiseDivide(Matrix m)
        {
            if (IsTheSameDimension(m))
            {
                Matrix oMat = new Matrix(this.m_iRows, this.m_iCols);
                for (int i = 0; i < this.m_iRows; i++)
                {
                    for (int j = 0; j < this.m_iCols; j++)
                    {
                        oMat.SetValueAt(i, j, GetValueAt(i, j) / m.GetValueAt(i, j));
                    }
                }
                return oMat;
            }
            else
            {
                throw new InvalidOperationException("The matrix m does not have the same dimensions than the current matrix!");
            }
        }

        public override Matrix ElementWiseMultiply(Matrix m)
        {
            if (IsTheSameDimension(m))
            {
                Matrix oMat = new Matrix(this.m_iRows, this.m_iCols);
                for (int i = 0; i < this.m_iRows; i++)
                {
                    for (int j = 0; j < this.m_iCols; j++)
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
            {
                throw new InvalidOperationException("The matrix m does not have the same dimensions than the current matrix!");
            }
        }

        public override Matrix ExpMatrix()
        {
            Matrix matrix = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = 0; j < matrix.m_iCols; j++)
                {
                    matrix.SetValueAt(i, j, Math.Exp(GetValueAt(i, j)));
                }
            }
            return matrix;
        }

        /**
         * This method returns a submatrix of this matrix. 
         * @param startRow the index of the first row (included)
         * @param endRow the index of the last row (included)
         * @param startColumn the index of the first column (included)
         * @param endColumn the index of the last column (included)
         * @return the submatrix in a Matrix instance
         */
        public override Matrix GetSubMatrix(int startRow, int endRow, int startColumn, int endColumn)
        {
            int iRows = endRow - startRow + 1;
            int iCols = endColumn - startColumn + 1;
            Matrix mat = new Matrix(iRows, iCols);
            for (int i = 0; i < iRows; i++)
            {
                for (int j = 0; j < iCols; j++)
                {
                    mat.SetValueAt(i, j, GetValueAt(startRow + i, startColumn + j));
                }
            }
            return mat;
        }

        /**
         * This method returns a sub matrix whose elements correspond to the indices listed in 
         * the row index list and the column index list.
         * 
         * @param rowIndex a List of integers (if null all the rows are selected)
         * @param columnIndex a List of integers (if null all the columns are selected)
         * @param sortIndices a boolean true to enable the sorting of the indices
         * @return a Matrix instance
         */
        public override Matrix GetSubMatrix(List<int> rowIndex, List<int> columnIndex, bool sortIndices)
        {
            if (rowIndex != null && rowIndex.Count != 0)
            {
                if (sortIndices)
                    rowIndex.Sort();
            }
            else
            {
                rowIndex = new List<int>();
                for (int i = 0; i < m_iRows; i++)
                {
                    rowIndex.Add(i);
                }
            }

            if (columnIndex != null && columnIndex.Count != 0)
            {
                if (sortIndices)
                    columnIndex.Sort();
            }
            else
            {
                columnIndex = new List<int>();
                for (int j = 0; j < m_iCols; j++)
                {
                    columnIndex.Add(j);
                }
            }

            Matrix outputMatrix = new Matrix(rowIndex.Count, columnIndex.Count);
            for (int i = 0; i < rowIndex.Count; i++)
            {
                for (int j = 0; j < columnIndex.Count; j++)
                {
                    outputMatrix.SetValueAt(i, j, GetValueAt(rowIndex[i], columnIndex[j]));
                }
            }

            return outputMatrix;
        }


        /**
         * This method returns a sub matrix whose elements correspond to the indices listed in 
         * the row index list and the column index list. <p>
         *
         * This method sorts the indices before constructing the sub matrices. So if rowIndex = {1,3,2},
         * the rows of resulting submatrix will correspond to rows 1, 2, 3 in this order. It is a proxy for 
         * getSubMatrix(rowIndex, columnIndex, true). 
         *  
         * @see Matrix#getSubMatrix(List, List, boolean)
         * @param rowIndex a List of integers (if null all the rows are selected)
         * @param columnIndex a List of integers (if null all the columns are selected)
         * @return a Matrix instance
         */
        public override Matrix GetSubMatrix(List<int> rowIndex, List<int> columnIndex)
        {
            if (rowIndex != null && rowIndex.Count != 0)
            {
                rowIndex.Sort();
            }
            else
            {
                rowIndex = new List<int>();
                for (int i = 0; i < m_iRows; i++)
                {
                    rowIndex.Add(i);
                }
            }

            if (columnIndex != null && columnIndex.Count != 0)
            {
                columnIndex.Sort();
            }
            else
            {
                columnIndex = new List<int>();
                for (int j = 0; j < m_iCols; j++)
                {
                    columnIndex.Add(j);
                }
            }

            Matrix outputMatrix = new Matrix(rowIndex.Count, columnIndex.Count);
            for (int i = 0; i < rowIndex.Count; i++)
            {
                for (int j = 0; j < columnIndex.Count; j++)
                {
                    outputMatrix.SetValueAt(i, j, GetValueAt(rowIndex[i], columnIndex[j]));
                }
            }

            return outputMatrix;
        }


        /**
         * This method checks whether this matrix is a diagonal matrix, i.e. with all its off-diagonal 
         * elements being equal to zero.
         * @return a boolean
         */
        public virtual bool IsDiagonalMatrix()
        {
            if (IsSquare())
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        if (i != j)
                        {
                            if (Math.Abs(GetValueAt(i, j)) != 0d)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Test whether the matrix is symmetric
        /// </summary>
        /// <returns>true if the matris is symmetric or false otherwise</returns>
        public virtual bool IsSymmetric()
        {
            bool valid = true;
            if (!IsSquare())
                return false;

            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = i + 1; j < m_iCols; j++)
                {
                    if (Math.Abs(GetValueAt(j, i)) < 1E-50)
                    {       // equal to 0
                        if (Math.Abs(GetValueAt(i, j)) > 1E-50)
                        {   // not equal to 0
                            valid = false;
                            break;
                        }
                    }
                    else
                    {
                        double ratio = GetValueAt(i, j) / GetValueAt(j, i);
                        if (Math.Abs(ratio - 1) > VERY_SMALL)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                if (!valid)
                    break;

            }
            return valid;
        }


        /**
         * Compute the logarithm of the elements of this matrix 
         * @return the results in a Matrix instance
         * @throws UnsupportedOperationException if one element of the matrix is smaller than or equal to 0
         */
        public override Matrix LogMatrix()
        {
            bool valid = true;       // default is valid
            Matrix matrix = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = 0; j < matrix.m_iCols; j++)
                {
                    if (GetValueAt(i, j) <= 0d)
                    {
                        valid = false;
                        break;
                    }
                    matrix.SetValueAt(i, j, Math.Log(GetValueAt(i, j)));
                }
                if (!valid)
                {
                    break;
                }
            }
            if (valid)
            {
                return matrix;
            }
            else
            {
                throw new InvalidOperationException("Matrix.logMatrix() : At least one argument value for the log function is smaller or equal to 0");
            }
        }

        /// <summary>
        /// Create a new matrix in which the current matrix represents the first diagonal block and matrix m represents the second diagonal block
        /// </summary>
        /// <param name="m">the matrix to be diagonally blocked</param>
        /// <returns>the result in a new Matrix instance</returns>
        public virtual Matrix MatrixDiagBlock(Matrix m)
        {
            int m1Row = m_iRows;
            int m1Col = m_iCols;
            int m2Row = m.m_iRows;
            int m2Col = m.m_iCols;
            Matrix matrix = new Matrix(m1Row + m2Row, m1Col + m2Col);
            matrix.SetSubMatrix(this, 0, 0);
            matrix.SetSubMatrix(m, m1Row, m1Col);
            return matrix;
        }


        /**
         * Compute a diagonal matrix from a row or a column vector.
         * @return the resulting matrix
         * @throws UnsupportedOperationException if this matrix is not a vector
         */
        public DiagonalMatrix MatrixDiagonal()
        {
            if (!IsRowVector() && !IsColumnVector())
            {
                throw new InvalidOperationException("Matrix.matrixDiagonal() : The input matrix is not a vector");
            }
            else
            {
                int dim;
                if (IsColumnVector())
                {
                    dim = m_iRows;
                }
                else
                {
                    dim = m_iCols;
                }
                DiagonalMatrix matrix = new DiagonalMatrix(dim);
                for (int i = 0; i < dim; i++)
                {
                    if (IsColumnVector())
                    {
                        matrix.SetValueAt(i, i, GetValueAt(i, 0));
                    }
                    else
                    {
                        matrix.SetValueAt(i, i, GetValueAt(0, i));
                    }
                }
                return matrix;
            }
        }

        /**
         * Create a new matrix which is the stack of this and matrix m. 
         * @param m the matrix to stack.
         * @param stackOver true if the stack is vertically or false if horizontally
         * @return the stacked matrix
         */
        public Matrix MatrixStack(Matrix m, bool stackOver)
        {
            int m1Row = m_iRows;
            int m1Col = m_iCols;
            int m2Row = m.m_iRows;
            int m2Col = m.m_iCols;
            if (m1Col == m2Col || m1Row == m2Row)
            {
                if (stackOver)
                {
                    Matrix matrix = new Matrix(m1Row + m2Row, m1Col);
                    matrix.SetSubMatrix(this, 0, 0);
                    matrix.SetSubMatrix(m, m1Row, 0);
                    return matrix;
                }
                else
                {
                    Matrix matrix = new Matrix(m1Row, m1Col + m2Col);
                    matrix.SetSubMatrix(this, 0, 0);
                    matrix.SetSubMatrix(m, 0, m1Col);
                    return matrix;
                }
            }
            else
            {
                throw new InvalidOperationException("Matrix m cannot be stacked on the current matrix because their dimensions do not match");
            }
        }

        /// <summary>
        /// Compute the matrix product of this x m
        /// </summary>
        /// <param name="m">a Matrix type object </param>
        /// <returns>a matrix type object that contains the result of the matrix multiplication</returns>
        /// <exception cref="InvalidOperationException">if the matrix dimensions are incompatible</exception>
        public virtual Matrix Multiply(Matrix m)
        {
            if (m_iCols != m.m_iRows)
            {
                throw new InvalidOperationException("The matrix m cannot multiply the current matrix for the number of rows is incompatible!");
            }
            else
            {
                Matrix mat = new Matrix(m_iRows, m.m_iCols);
                for (int i_this = 0; i_this < m_iRows; i_this++)
                {
                    for (int j_m = 0; j_m < m.m_iCols; j_m++)
                    {
                        double sum = 0d;
                        for (int j_this = 0; j_this < m_iCols; j_this++)
                        {
                            int i_m = j_this;
                            sum += GetValueAt(i_this, j_this) * m.GetValueAt(i_m, j_m);
                        }
                        mat.SetValueAt(i_this, j_m, sum);
                    }
                }
                return mat;
            }
        }

        /**
         * Reset all the elements of this Matrix instance to 0.
         */
        public void ResetMatrix()
        {
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    SetValueAt(i, j, 0d);
                }
            }
        }

        public override Matrix ScalarAdd(double d)
        {
            Matrix mat = new(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    mat.SetValueAt(i, j, GetValueAt(i, j) + d);
                }
            }
            return mat;
        }

        public override Matrix ScalarMultiply(double d)
        {
            Matrix mat = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    mat.SetValueAt(i, j, GetValueAt(i, j) * d);
                }
            }
            return mat;
        }


        public override void SetSubMatrix(Matrix m, int i, int j)
        {
            if (this is SymmetricMatrix)
            {
                throw new InvalidOperationException("This Matrix instance does not support the setSubMatrix method!");
            }
            else
            {
                for (int ii = 0; ii < m.m_iRows; ii++)
                {
                    for (int jj = 0; jj < m.m_iCols; jj++)
                    {
                        SetValueAt(i + ii, j + jj, m.GetValueAt(ii, jj));
                    }
                }
            }
        }

        /// <summary>
        /// Create a square symmetric matrix from a vector.<br></br>
        /// Checks are implemented to make sure the vector has the appropriate number of elements.
        /// </summary>
        /// <returns>the resulting matrix</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public SymmetricMatrix SquareSym()
        {
            if (!IsColumnVector())
            {
                throw new InvalidOperationException("The current matrix is not a column vector!");
            }
            else
            {
                double numberElem = m_iRows;
                double numberRow = (-1.0 + Math.Sqrt(1.0 + 8 * numberElem)) * 0.5;
                int nbRow = Convert.ToInt32(numberRow);
                if (Math.Abs(numberRow - nbRow) > Matrix.VERY_SMALL)
                {   // check if numberRow is an integer, if not it means the matrix is not square
                    throw new InvalidOperationException("The number of elements contained in the imput column vector is not appropriate to transform the matrix into a square symmetric matrix!");
                }
                else
                {
                    SymmetricMatrix matrix = new(nbRow);
                    int pointer = 0;
                    for (int i = 0; i < nbRow; i++)
                    {
                        Matrix subMat = GetSubMatrix(pointer, pointer + i, 0, 0);
                        for (int j = 0; j < subMat.m_iRows; j++)
                        {
                            matrix.SetValueAt(j, i, subMat.GetValueAt(j, 0));
                        }
                        pointer += i + 1;
                    }
                    return matrix;
                }
            }
        }

        public override Matrix Subtract(Matrix m)
        {
            if (!IsTheSameDimension(m))
            {
                throw new InvalidOperationException("This instance and the Matrix m are not of the same dimension!");
            }
            Matrix mat = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    mat.SetValueAt(i, j, GetValueAt(i, j) - m.GetValueAt(i, j));
                }
            }
            return mat;
        }

        /**
         * Compute the trace of the matrix.
         * @return a double
         */
        public double GetTrace()
        {
            if (!IsSquare())
            {
                throw new InvalidOperationException("The trace operation requires the matrix to be square!");
            }
            double sum = 0;
            for (int i = 0; i < m_iRows; i++)
            {
                sum += GetValueAt(i, i);
            }
            return sum;
        }

        public override Matrix Transpose()
        {
            Matrix matrix = new(m_iCols, m_iRows);
            for (int i = 0; i < m_iRows; i++)
                for (int j = 0; j < m_iCols; j++)
                    matrix.SetValueAt(j, i, GetValueAt(i, j));
            return matrix;
        }

        /// <summary>
        /// Compute the power of the seed by the elements of the matrix.<br></br> 
        /// For example, if the first element of this matrix is 2, the first element 
        /// of the resulting matrix will be seed ^ 2.
        /// </summary>
        /// <param name="seed">a double</param>
        /// <returns>a Matrix instance</returns>
        public virtual Matrix PowMatrix(double seed)
        {
            Matrix matrix = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = 0; j < matrix.m_iCols; j++)
                {
                    matrix.SetValueAt(i, j, Math.Pow(seed, GetValueAt(i, j)));
                }
            }
            return matrix;
        }

        public override Matrix ElementWisePower(double power)
        {
            Matrix matrix = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < matrix.m_iRows; i++)
            {
                for (int j = 0; j < matrix.m_iCols; j++)
                {
                    matrix.SetValueAt(i, j, Math.Pow(GetValueAt(i, j), power));
                }
            }
            return matrix;
        }


        /**
         * Repeat this matrix a given number of times in each dimension.
         * @param nrow the number of times to repeat in row-wise direction
         * @param ncol the number of times to repeat in column-wise direction
         * @return the resulting matrix
         */
        public Matrix Repeat(int nrow, int ncol)
        {
            Matrix resultingMatrix = new Matrix(m_iRows * nrow, m_iCols * ncol);
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    resultingMatrix.SetSubMatrix(this, i * m_iRows, j * m_iCols);
                }
            }
            return resultingMatrix;
        }

        /**
         * Remove some elements in a particular matrix and create a row vector. 
         * @param index is the index of the elements to be removed
         * @return a row vector
         */
        public Matrix RemoveElements(List<int> index)
        {
            Matrix oMat = new Matrix(1, m_iRows * m_iCols - index.Count);
            int pointer = 0;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (!index.Contains(i * m_iCols + j))
                    {
                        oMat.SetValueAt(0, pointer, GetValueAt(i, j));
                        pointer++;
                    }
                }
            }
            return oMat;
        }

        /**
         * Return the elements defined by the List indices in a row vector.
         * @param indices a List of indices
         * @return a row vector
         */
        public Matrix GetElements(List<int> indices)
        {
            Matrix oMat = new Matrix(1, indices.Count);
            int pointer = 0;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (indices.Contains(i * m_iCols + j))
                    {
                        oMat.SetValueAt(0, pointer, GetValueAt(i, j));
                        pointer++;
                    }
                }
            }
            return oMat;
        }

        /**
         * Replace the elements of the matrix designated through the indices by the values
         * in the row vector m.
         * @param indices a List of Integer representing the indices
         * @param m a Matrix instance
         */
        public void SetElements(List<int> indices, Matrix m)
        {
            if (this is SymmetricMatrix)
            {
                throw new InvalidOperationException("This Matrix instance does not support the setElement method!");
            }
            if (!m.IsColumnVector())
            {
                throw new ArgumentException("Parameter m must be a row vector!");
            }
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (indices.Contains(i * m_iCols + j))
                    {
                        SetValueAt(i, j, m.GetValueAt(indices.IndexOf(i * m_iCols + j), 0));
                    }
                }
            }
        }

        /**
         * Add the elements of the parameter matrix to those designated through the indices.
         * @param indices a List of Integer representing the indices
         * @param m a Matrix instance
         */
        public void AddElementsAt(List<int> indices, Matrix m)
        {
            if (this is SymmetricMatrix)
            {
                throw new InvalidOperationException("This Matrix instance does not support the addElementsAt method!");
            }
            if (!m.IsColumnVector())
            {
                throw new ArgumentException("Parameter m must be a row vector!");
            }
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (indices.Contains(i * m_iCols + j))
                    {
                        double newValue = GetValueAt(i, j) + m.GetValueAt(indices.IndexOf(i * m_iCols + j), 0);
                        SetValueAt(i, j, newValue);
                    }
                }
            }
        }

        /**
         * Return a List of integers, which represent the index of the elements
         * that are equal to parameter d. The index is calculated as i * m_iCols + j.
         * @param d the value that is checked for
         * @return a List of integers
         */
        public List<int> GetLocationIndex(double d)
        {

            List<int> list = new List<int>();
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    if (Math.Abs(GetValueAt(i, j) - d) < VERY_SMALL)
                    {
                        list.Add(i * m_iCols + j);
                    }
                }
            }
            return list;
        }

        /**
         * Compute the sum of all the elements in the Matrix instance.
         * @return a double
         */
        public double GetSumOfElements()
        {
            double sum = 0d;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    sum += GetValueAt(i, j);
                }
            }
            return sum;
        }

        /**
         * Compute the sum of the elements of a submatrix. The submatrix bounds are determined
         * through the parameters.
         * @param startRow the index of the starting row
         * @param endRow the index of the ending row 
         * @param startColumn the index of the starting column
         * @param endColumn the index of the ending column
         * @return the sum (double)
         */
        public double GetSumOfElements(int startRow, int endRow, int startColumn, int endColumn)
        {
            if (endRow >= this.m_iRows || endColumn >= this.m_iCols)
            {
                throw new ArgumentException("The specified end row or end column exceeds the capacity of the matrix!");
            }
            else if (startRow < 0 || startRow > endRow)
            {
                throw new ArgumentException("The specified start row is either negative or larger than the end row!");
            }
            else if (startColumn < 0 || startColumn > endColumn)
            {
                throw new ArgumentException("The specified start column is either negative or larger than the end column!");
            }
            double sum = 0d;
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    sum += GetValueAt(i, j);
                }
            }
            return sum;
        }

        /// <summary>
        /// Create a Matrix instance that contains the absolute values of the original matrix
        /// </summary>
        /// <returns>a Matrix instance that contains the absolute values</returns>
        public virtual Matrix GetAbsoluteValue()
        {
            Matrix oMat = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    oMat.SetValueAt(i, j, Math.Abs(GetValueAt(i, j)));
                }
            }
            return oMat;
        }

        /**
         * Return the number of elements in a Matrix object.
         * @return the number of elements (integer)
         */
        public int GetNumberOfElements()
        {
            return m_iRows * m_iCols;
        }

        /// <summary>
        /// Calculate the Kronecker product of this by the m Matrix object
        /// </summary>
        /// <param name="m">a Matrix instance</param>
        /// <returns>the resulting product (a Matrix instance)</returns>
        public virtual Matrix GetKroneckerProduct(Matrix m)
        {
            Matrix result = new(m_iRows * m.m_iRows, m_iCols * m.m_iCols);
            for (int i1 = 0; i1 < m_iRows; i1++)
                for (int j1 = 0; j1 < m_iCols; j1++)
                    for (int i2 = 0; i2 < m.m_iRows; i2++)
                        for (int j2 = 0; j2 < m.m_iCols; j2++)
                            result.SetValueAt(i1 * m.m_iRows + i2, j1 * m.m_iCols + j2, GetValueAt(i1, j1) * m.GetValueAt(i2, j2));
            return result;
        }

        protected internal Matrix GetIsserlisMatrixOnlyForTestPurpose()
        {
            if (!IsSymmetric())
                throw new InvalidOperationException("Matrix.getIsserlisMatrix: this matrix is not symmetric!");
            else
            {
                Matrix output = new Matrix(m_iRows * m_iRows, m_iCols * m_iCols);
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
                                if (indexRow != indexCol)
                                {
                                    output.SetValueAt(indexCol, indexRow, covariance);
                                }
                            }
                        }
                    }
                }

                return output;
            }
        }


        public override Matrix Clone()
        {
            Matrix oMat = new(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
                for (int j = 0; j < m_iCols; j++)
                    oMat.SetValueAt(i, j, GetValueAt(i, j));
            return oMat;
        }

        protected override string ConvertArrayToString(int rowIndex)
        {
            StringBuilder outputString = new StringBuilder();
            for (int j = 0; j < m_iCols; j++)
            {
                if (j > 0)
                {
                    outputString.Append(" ");
                }
                double absValue = Math.Abs(GetValueAt(rowIndex, j));
                if (absValue > 0.1 && absValue < 1E3)
                    outputString.Append("[" + GetValueAt(rowIndex, j).ToString("{0:0.00}") + "]");
                else
                    outputString.Append("[" + GetValueAt(rowIndex, j).ToString("0.##E0") + "]");
            }
            return outputString.ToString();
        }

        /**
         * Return the LU decomposition of this Matrix instance. The diagonal in the lower triangle is 1.
         * @return an array of two matrices, the first and the second being the lower and the upper triangle, respectively
         * @throws UnsupportedOperationException if the matrix is not square
         */
        public Matrix[] getLUDecomposition()
        {
            Matrix[] outputMatrices = new Matrix[2];
            if (!IsSquare())
            {
                throw new InvalidOperationException("Matrix.getLUDecomposition(): The matrix is not square!");
            }
            else
            {
                Matrix l = new Matrix(m_iRows, m_iCols);
                Matrix u = new Matrix(m_iRows, m_iCols);
                for (int i = 0; i < m_iRows; i++)
                {
                    l.SetValueAt(i, i, 1d);
                    for (int j = i; j < m_iRows; j++)
                    {
                        double sum = 0;
                        for (int s = 0; s <= i - 1; s++)
                        {
                            sum += l.GetValueAt(i, s) * u.GetValueAt(s, j);
                        }
                        u.SetValueAt(i, j, GetValueAt(i, j) - sum);
                    }
                    for (int iii = i + 1; iii < m_iRows; iii++)
                    {
                        double sum = 0;
                        for (int s = 0; s <= i - 1; s++)
                        {
                            sum += l.GetValueAt(iii, s) * u.GetValueAt(s, i);
                        }
                        if (u.GetValueAt(i, i) == 0d)
                        {
                            throw new InvalidOperationException("The determinant cannot be calculated because of a division by 0!");
                        }
                        l.SetValueAt(iii, i, (GetValueAt(iii, i) - sum) / u.GetValueAt(i, i));
                    }
                }
                outputMatrices[0] = l;
                outputMatrices[1] = u;
                return outputMatrices;
            }
        }

        private void Swap(int i, int j, bool columnWise)
        {
            if (columnWise)
            {
                if (i >= m_iCols || j >= m_iCols)
                {
                    throw new InvalidOperationException("Columns cannot be swapped as their indices are out of bound!");
                }
                else
                {
                    double d;
                    for (int k = 0; k < m_iRows; k++)
                    {
                        d = GetValueAt(k, i);
                        SetValueAt(k, i, GetValueAt(k, j));
                        SetValueAt(k, j, d);
                    }
                }
            }
            else
            {
                if (i >= m_iRows || j >= m_iRows)
                {
                    throw new InvalidOperationException("Columns cannot be swapped as their indices are out of bound!");
                }
                else
                {
                    double d;
                    for (int k = 0; k < m_iCols; k++)
                    {
                        d = GetValueAt(i, k);
                        SetValueAt(i, k, GetValueAt(j, k));
                        SetValueAt(j, k, d);
                    }
                }
            }
        }

        protected void SwapAlongTheDiagonal(int i, int j)
        {
            if (!IsSquare())
            {
                throw new InvalidOperationException("The matrix is not square!");
            }
            if (i >= m_iCols || j >= m_iCols)
            {
                throw new InvalidOperationException("The index is out of bound!");
            }
            else
            {
                Swap(i, j, true);
                Swap(j, i, false);
            }
        }


        /**
         * Return a list of indices that define the blocks in the matrix.
         * @return a List of List of Integers
         */
        protected List<List<int>> GetBlockConfiguration()
        {
            if (!IsSquare())
            {
                throw new InvalidOperationException("The matrix is not square!");
            }

            List<int> remainingIndex = new List<int>();
            for (int i = 0; i < m_iCols; i++)
            {
                remainingIndex.Add(i);
            }

            List<List<int>> blocks = new List<List<int>>();

            if (!IsSymmetric())
            {
                blocks.Add(remainingIndex);
                return blocks;
            }
            else
            {
                List<int> potentialBlock;
                while (remainingIndex.Count != 0)
                {
                    int i = 0;
                    potentialBlock = new List<int>();
                    potentialBlock.Add(remainingIndex[i]);
                    while (i < potentialBlock.Count)
                    {
                        int indexI = potentialBlock[i];
                        for (int j = remainingIndex.IndexOf(indexI) + 1; j < remainingIndex.Count; j++)
                        {
                            int indexJ = remainingIndex[j];
                            if (GetValueAt(indexI, indexJ) != 0)
                            {
                                if (!potentialBlock.Contains(indexJ))
                                {
                                    potentialBlock.Add(indexJ);
                                }
                                for (int k = i + 1; k < j; k++)
                                {
                                    int indexK = remainingIndex[k];
                                    if (GetValueAt(indexK, indexJ) != 0)
                                    {
                                        if (!potentialBlock.Contains(indexK))
                                        {
                                            potentialBlock.Add(indexK);
                                        }
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    blocks.Add(potentialBlock);
                    remainingIndex.RemoveAll(p => potentialBlock.Contains(p));
                }
                return blocks;
            }
        }


        /**
         * Compute the minor of this matrix, i.e. the determinant of the Matrix that 
         * contains all the elements of the original matrix except those in row i and column j.
         * @param i the index of the row to be omitted
         * @param j the index of the column to be omitted
         * @return the minor
         */
        public double GetMinor(int i, int j)
        {
            if (i >= m_iRows)
            {
                throw new ArgumentException("The index i is not within the bound of this matrix!");
            }
            else if (j >= m_iCols)
            {
                throw new ArgumentException("The index j is not within the bound of this matrix!");
            }
            else if (GetNumberOfElements() == 1)
            {
                throw new InvalidOperationException("The matrix only has one element!");
            }
            else
            {
                Matrix m = new Matrix(m_iRows - 1, m_iCols - 1);
                int index_i = 0;
                for (int ii = 0; ii < m_iRows; ii++)
                {       // iterate in the current matrix
                    if (ii != i)
                    {
                        int index_j = 0;
                        for (int jj = 0; jj < m_iCols; jj++)
                        {
                            if (jj != j)
                            {
                                m.SetValueAt(index_i, index_j, GetValueAt(ii, jj));
                                index_j++;
                            }
                        }
                        index_i++;
                    }
                }
                return m.GetDeterminant();
            }
        }

        /**
         * Compute the cofactor of this matrix with respect to the element i,j.
         * @param i the row index of the element
         * @param j the column index of the element
         * @return the cofactor matrix
         */
        public double GetCofactor(int i, int j)
        {
            double minor = GetMinor(i, j);
            double multiplicator = 1d;
            if ((i + j) % 2 != 0)
            {
                multiplicator = -1d;
            }
            return minor * multiplicator;
        }


        /**
         * Compute the determinant of this matrix using Laplace's method for small matrices and LU decomposition
         * for larger matrices.
         * @return a double
         * @throws UnsupportedOperationException if the matrix is not square
         */
        public double GetDeterminant()
        {
            double determinant = 0;
            if (!IsSquare())
            {
                throw new InvalidOperationException("The matrix is not square!");
            }
            else if (m_iRows == 1)
            {
                return GetValueAt(0, 0);
            }
            else if (m_iRows == 2)
            {
                return GetValueAt(0, 0) * GetValueAt(1, 1) - GetValueAt(0, 1) * GetValueAt(1, 0);
            }
            else if (m_iRows <= SizeBeforeSwitchingToLUDecompositionInDeterminantCalculation)
            {
                for (int j = 0; j < m_iRows; j++)
                {
                    if (GetValueAt(0, j) != 0d)
                    {
                        determinant += GetValueAt(0, j) * GetCofactor(0, j);
                    }
                }
            }
            else
            {
                Matrix triangle = getLUDecomposition()[1];
                determinant = 1d;
                for (int i = 0; i < triangle.m_iRows; i++)
                {
                    determinant *= triangle.GetValueAt(i, i);
                }
            }
            return determinant;
        }

        /**
         * Compute the adjugate matrix of this matrix.
         * @return a Matrix instance
         */
        public Matrix GetAdjugateMatrix()
        {
            Matrix adjugate = new Matrix(m_iRows, m_iCols);
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    adjugate.SetValueAt(j, i, GetCofactor(i, j));       // i and j are inversed for adjugate to ensure the transposition
                }
            }
            return adjugate;
        }

        internal virtual Matrix GetInternalInverseMatrix()
        {
            if (m_iRows == 1)
            {
                Matrix output = new(1, 1);
                output.SetValueAt(0, 0, 1d / GetValueAt(0, 0));
                return output;
            }
            else if (m_iRows > SizeBeforeSwitchingToLUDecompositionInDeterminantCalculation)
            {
                int index = m_iCols / 2;
                Matrix output = new (m_iRows, m_iCols);
                Matrix a = GetSubMatrix(0, index - 1, 0, index - 1);
                Matrix b = GetSubMatrix(0, index - 1, index, m_iCols - 1);
                Matrix c = GetSubMatrix(index, m_iRows - 1, 0, index - 1);
                Matrix d = GetSubMatrix(index, m_iRows - 1, index, m_iCols - 1);
                Matrix invD = d.GetInternalInverseMatrix();
                Matrix tmp = b.Multiply(invD).Multiply(c);
                Matrix invComplement = a.Subtract(tmp).GetInternalInverseMatrix();
                output.SetSubMatrix(invComplement, 0, 0);
                output.SetSubMatrix(invComplement.Multiply(b).Multiply(invD).ScalarMultiply(-1d), 0, index);
                output.SetSubMatrix(invD.Multiply(c).Multiply(invComplement).ScalarMultiply(-1d), index, 0);
                output.SetSubMatrix(invD.Multiply(c).Multiply(invComplement).Multiply(b).Multiply(invD).Add(invD), index, index);
                return output;
            }
            else
            {
                double determinant = GetDeterminant();
                if (determinant == 0)
                    throw new InvalidOperationException("The matrix cannot be inverted as its determinant is equal to 0!");
                else
                    return GetAdjugateMatrix().ScalarMultiply(1d / determinant);
            }
        }


        /**
         * Ensure that no elements in the matrix are lower than value by setting them to value if needed. <p>
         * This method changes the matrix in place 
         * @param value the threshold. Lower values are set to this value.
         */
        public void ClampIfLowerThan(double value)
        {
            if (IsNewImplementationForColumnVector())
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        this.m_afData[j][i] = this.m_afData[j][i] < value ? value : this.m_afData[j][i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        this.m_afData[i][j] = this.m_afData[i][j] < value ? value : this.m_afData[i][j];
                    }
                }
            }
        }

        /**
         * Ensure that no elements in the matrix are higher than value by setting them to value if needed.<p>
         * This method changes the matrix in place 
         * @param value the threshold. Higher values are set to this value.
         */
        public void ClampIfHigherThan(double value)
        {
            if (IsNewImplementationForColumnVector())
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        this.m_afData[j][i] = this.m_afData[j][i] > value ? value : this.m_afData[j][i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        this.m_afData[i][j] = this.m_afData[i][j] > value ? value : this.m_afData[i][j];
                    }
                }
            }
        }

        /// <summary>
        /// Compute the inverse of this matrix
        /// </summary>
        /// <returns>the inverse matrix</returns>
        public virtual Matrix GetInverseMatrix()
        {
            if (IsDiagonalMatrix()) // procedure for diagonal matrices
                return DiagonalMatrix.ForceConversionToDiagonalMatrix(this).GetInverseMatrix();
            List<List<int>> indices = GetBlockConfiguration();
            if (indices.Count == 1)
                return GetInternalInverseMatrix();
            else
            {
                Matrix inverseMatrix = new(m_iRows, m_iCols);
                foreach (List<int> blockIndex in indices)
                {
                    Matrix invSubMatrix = GetSubMatrix(blockIndex, blockIndex).GetInternalInverseMatrix();
                    for (int i = 0; i < blockIndex.Count; i++)
                        for (int j = 0; j < blockIndex.Count; j++)
                            inverseMatrix.SetValueAt(blockIndex[i], blockIndex[j], invSubMatrix.GetValueAt(i, j));
                }
                return inverseMatrix;
            }

        }

        /**
         * Create an identity matrix of dimension i.
         * @param dim the dimension of the matrix
         * @return a Matrix instance
         */
        public static DiagonalMatrix GetIdentityMatrix(int dim)
        {
            if (dim <= 0)
            {
                throw new ArgumentException("The dim argument must be larger than 0!");
            }
            DiagonalMatrix mat = new DiagonalMatrix(dim);
            for (int i = 0; i < dim; i++)
            {
                mat.SetValueAt(i, i, 1d);
            }
            return mat;
        }

        public override bool Equals(object? obj)
        {
            if (base.Equals(obj))
                return true;
            else if (obj is Matrix)
            {
                Matrix mat = (Matrix)obj;
                if (mat.m_iCols != m_iCols || mat.m_iRows != m_iRows)
                    return false;
                else
                {
                    for (int i = 0; i < m_iRows; i++)
                        for (int j = 0; j < m_iCols; j++)
                            if (GetValueAt(i, j) != mat.GetValueAt(i, j))
                                return false;
                    return true;
                }
            }
            else
                return false;
        }
    }
}