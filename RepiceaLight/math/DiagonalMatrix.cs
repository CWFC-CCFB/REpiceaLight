using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.math
{
    public class DiagonalMatrix : SymmetricMatrix
    {


    public DiagonalMatrix(int size) : base(size) 
    {
    }


        protected override double[][] ContructInternalArray(int iRows, int iCols)
        {
            double[][] mainArray = [new double[iRows]];
            return mainArray;
        }

        public override bool AnyElementNaN()
    {
        for (int i = 0; i < this.m_iRows; i++)
            if (Double.IsNaN(this.GetValueAt(i, i)))
                return true;
        return false;
    }


    public override void SetValueAt(int i, int j, double value)
    {
        if (j != i)
            throw new InvalidOperationException("The DiagonalMatrix instance only allows for setting the values on the diagonal!");
        else
            m_afData[0][i] = value;
    }

    public override double GetValueAt(int i, int j)
    {
        if (i < 0 || i >= this.m_iRows)
            throw new InvalidOperationException("The index i exceeds the dimension of the Matrix instance!");
        if (j < 0 || j >= this.m_iCols)
            throw new InvalidOperationException("The index i exceeds the dimension of the Matrix instance!");
        return j == i ? m_afData[0][i] : 0d;
    }

    public override bool IsDiagonalMatrix()
    {
        return true;
    }

    public override DiagonalMatrix ElementWiseDivide(Matrix m)
    {
        if (IsTheSameDimension(m))
        {
            DiagonalMatrix oMat = new(m_iRows);
            for (int i = 0; i < this.m_iRows; i++)
                oMat.SetValueAt(i, i, GetValueAt(i, i) / m.GetValueAt(i, i));
            return oMat;
        }
        else
            throw new InvalidOperationException("The matrix m does not have the same dimensions than the current matrix!");
    }

    public override DiagonalMatrix ElementWiseMultiply(Matrix m)
    {
        if (IsTheSameDimension(m))
        {
            DiagonalMatrix oMat = new DiagonalMatrix(m_iRows);
            for (int i = 0; i < this.m_iRows; i++)
                oMat.SetValueAt(i, i, GetValueAt(i, i) * m.GetValueAt(i, i));
            return oMat;
        }
        else
            throw new InvalidOperationException("The matrix m does not have the same dimensions than the current matrix!");
    }

    public override SymmetricMatrix LogMatrix()
    {
        throw new InvalidOperationException("The DiagonalMatrix class does not support the logMatrix method!");
    }


    public override DiagonalMatrix ScalarMultiply(double d)
    {
        DiagonalMatrix mat = new(m_iRows);
        for (int i = 0; i < m_iRows; i++)
            mat.SetValueAt(i, i, GetValueAt(i, i) * d);
        return mat;
    }

    public override DiagonalMatrix ElementWisePower(double power)
    {
        DiagonalMatrix matrix = new(m_iRows);
        for (int i = 0; i < matrix.m_iRows; i++)
            matrix.SetValueAt(i, i, Math.Pow(GetValueAt(i, i), power));
        return matrix;
    }


    public override DiagonalMatrix GetAbsoluteValue()
    {
        DiagonalMatrix oMat = new DiagonalMatrix(m_iRows);
        for (int i = 0; i < m_iRows; i++)
            oMat.SetValueAt(i, i, Math.Abs(GetValueAt(i, i)));
        return oMat;
    }

    public override DiagonalMatrix GetLowerCholTriangle()
    {
        DiagonalMatrix matrix = new(m_iRows);
        for (int i = 0; i < m_iRows; i++)
            matrix.SetValueAt(i, i, Math.Sqrt(GetValueAt(i, i)));
        return matrix;
    }

    public override DiagonalMatrix Clone()
    {
        DiagonalMatrix oMat = new(m_iRows);
        for (int i = 0; i < m_iRows; i++)
            oMat.SetValueAt(i, i, GetValueAt(i, i));
        return oMat;
    }


    public override Matrix Multiply(Matrix m)
    {
        if (m_iCols != m.m_iRows)
            throw new InvalidOperationException("The matrix m cannot multiply the current matrix for the number of rows is incompatible!");
        else
        {
            if (m.Equals(this))
            {   // multiplied by itself yields a SymmetricMatrix instance
                DiagonalMatrix mat = new DiagonalMatrix(m_iRows);
                for (int i = 0; i < m_iRows; i++)
                {
                    double originalValue = GetValueAt(i, i);
                    mat.SetValueAt(i, i, originalValue * originalValue);
                }
                return mat;
            }
            else
                return base.Multiply(m);
        }
    }


    internal override DiagonalMatrix GetInternalInverseMatrix()
    {
        DiagonalMatrix m = this.ElementWisePower(-1);
        return m;
    }


    public override Matrix MatrixDiagBlock(Matrix m)
    {
        if (m.IsDiagonalMatrix())
        {
            int m1Row = m_iRows;
            int m2Row = m.m_iRows;
            DiagonalMatrix matrix = new(m1Row + m2Row);
            for (int i = 0; i < m1Row; i++)
                matrix.SetValueAt(i, i, this.GetValueAt(i, i));
            for (int i = 0; i < m2Row; i++)
                matrix.SetValueAt(i + m1Row, i + m1Row, m.GetValueAt(i, i));
            return matrix;
        }
        else
            return base.MatrixDiagBlock(m);
    }


    public override DiagonalMatrix GetInverseMatrix()
    {
        return GetInternalInverseMatrix();
    }

    internal static DiagonalMatrix ForceConversionToDiagonalMatrix(Matrix m)
    {
        if (m is DiagonalMatrix) {
            return (DiagonalMatrix)m;
        } else
        {
            if (!m.IsDiagonalMatrix())
                throw new InvalidOperationException("Some off diagonal elements are different from 0!");
            else
            {
                DiagonalMatrix sm = new(m.m_iRows);
                for (int i = 0; i < m.m_iRows; i++)
                {
                    sm.SetValueAt(i, i, m.GetValueAt(i, i));
                }
                return sm;
            }
        }

    }


}

}
