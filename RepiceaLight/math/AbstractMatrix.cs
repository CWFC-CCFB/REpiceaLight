using System.Text;

namespace REpiceaLight.math
{
    public abstract class AbstractMatrix<P> : ICloneable where P : AbstractMatrix<P>
    {


        public readonly int m_iRows;
        public readonly int m_iCols;

        protected AbstractMatrix(int iRows, int iCols)
        {
            if (iRows <= 0 || iCols <= 0)
            {
                throw new ArgumentException("The number of rows or columns must be equal to or greater than 1!");
            }
            m_iRows = iRows;
            m_iCols = iCols;
        }

        /// <summary>
        /// Add matrix m to the current matrix.
        /// </summary>
        /// <param name="m">the matrix to be added</param>
        /// <returns>the result in a new Matrix instance</returns>
        public abstract P Add(P m);

        /// <summary>
        /// Compute the exponential of the elements of this matrix 
        /// </summary>
        /// <returns>the results in a Matrix instance</returns>
        public abstract P ExpMatrix();


        /**
         * This method returns a submatrix of this matrix. 
         * @param startRow the index of the first row (included)
         * @param endRow the index of the last row (included)
         * @param startColumn the index of the first column (included)
         * @param endColumn the index of the last column (included)
         * @return the submatrix in a Matrix instance
         */
        public abstract P GetSubMatrix(int startRow, int endRow, int startColumn, int endColumn);

        /**
         * This method returns a sub matrix whose elements correspond to the indices listed in 
         * the row index list and the column index list.
         * 
         * @param rowIndex a List of integers (if null all the rows are selected)
         * @param columnIndex a List of integers (if null all the columns are selected)
         * @param sortIndices a boolean true to enable the sorting of the indices
         * @return a Matrix instance
         */
        public abstract P GetSubMatrix(List<int> rowIndex, List<int> columnIndex, bool sortIndices);


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
        public abstract P GetSubMatrix(List<int> rowIndex, List<int> columnIndex);


        /// <summary>
        /// Compute the elements of the matrix at a given power
        /// </summary>
        /// <param name="power">a double</param>
        /// <returns>a Matrix instance</returns>
        public abstract P ElementWisePower(double power);

        /// <summary>
        /// Compute the elementwise product of this by m
        /// </summary>
        /// <param name="m">the matrix that contains the elements to be multiplied with.</param>
        /// <returns>a Matrix instance</returns>
        public abstract P ElementWiseMultiply(P m);

        /**
         * This method checks if this is a column vector
         * @return a boolean that is true if this is a column vector
         */
        public bool IsColumnVector() { return m_iCols == 1; }

        /**
         * This method checks if this is a row vector
         * @return a boolean that is true if this is a row vector
         */
        public bool IsRowVector() { return m_iRows == 1; }

        /**
         * This method checks if this is a square matrix
         * @return true if the matrix is square or false otherwise
         */
        public bool IsSquare() { return m_iRows == m_iCols; }

        /**
         * This method checks whether or not this and m have the same dimensions
         * @param m an AbstractMatrix-derived instance
         * @return boolean
         */
        public bool IsTheSameDimension(P m)
        {
            bool output = false;
            if (m_iCols == m.m_iCols)
            {
                if (m_iRows == m.m_iRows)
                {
                    output = true;
                }
            }
            return output;
        }

        /// <summary>
        /// Add the scalar to all the elements of the current matrix
        /// </summary>
        /// <param name="d">the scalar to be added</param>
        /// <returns>the result in a new Matrix instance</returns>
        public abstract P ScalarAdd(double d);

        /// <summary>
        /// Multiply the elements of the current matrix by the scalar
        /// </summary>
        /// <param name="d">the multiplier</param>
        /// <returns>the result in a new Matrix instance</returns>
        public abstract P ScalarMultiply(double d);


        /**
         * Replace some elements of the matrix by those that are contained in matrix m.
         * @param m a Matrix instance 
         * @param i the row index of the first element to be changed
         * @param j the column index of the first element to be changed
         */
        public abstract void SetSubMatrix(P m, int i, int j);


        /**
         * Compute the logarithm of the elements of this matrix 
         * @return the results in a Matrix instance
         * @throws UnsupportedOperationException if one element of the matrix is smaller than or equal to 0
         */
        public abstract P LogMatrix();

        /// <summary>
        /// Subtract matrix m from this matrix
        /// </summary>
        /// <param name="m">the matrix to be subtracted</param>
        /// <returns>the result in a new Matrix instance</returns>
        public abstract P Subtract(P m);


        /// <summary>
        /// Create a transposed matrix
        /// </summary>
        /// <returns>the transposed matrix in a new Matrix instance</returns>
        public abstract P Transpose();



        /**
         * Returns a representation of the matrix content.
         */
        public sealed override string ToString()
        {
            StringBuilder outputString = new StringBuilder();
            outputString.Append("{");
            for (int i = 0; i < m_iRows; i++)
            {
                outputString.Append(ConvertArrayToString(i));
                if (i == m_iRows - 1)
                {
                    outputString.Append("}");
                }
                else
                {
                    if (IsColumnVector())
                    {
                        outputString.Append(", ");
                    }
                    else
                    {
                        outputString.Append(", \n");
                    }
                }
                if (outputString.Length > 5000)
                {
                    outputString.Append("...");
                    break;
                }
            }
            return outputString.ToString();
        }

        /**
         * Convert a particular row of the matrix into a string.
         * @param rowIndex the index of the row to be converted
         * @return a String instance
         */
        protected abstract string ConvertArrayToString(int rowIndex);

        /// <summary>
        /// Produce a clone of the current Matrix instance
        /// </summary>
        /// <returns>a new Matrix instance</returns>
        public abstract object Clone();
    }
}
