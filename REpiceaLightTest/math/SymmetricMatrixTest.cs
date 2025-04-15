using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.math
{
    [TestClass]
    public sealed class SymmetricMatrixTest
    {

        static SymmetricMatrix SymMat;

        [TestInitialize]
        public void Initialize()
        {
            SymmetricMatrix sm = new(3);
            sm.SetValueAt(0, 0, 1);
            sm.SetValueAt(0, 1, 2);
            sm.SetValueAt(0, 2, 3);
            sm.SetValueAt(1, 1, 4);
            sm.SetValueAt(1, 2, 5);
            sm.SetValueAt(2, 2, 6);
            SymMat = sm;
        }

        [TestMethod]
        public void Test01InternalArrayConstruction()
        {
            SymmetricMatrix sm = new(4);
            Assert.AreEqual(4, sm.m_afData[0].Length);
            Assert.AreEqual(1, sm.m_afData[3].Length);
        }

        [TestMethod]
        public void Test02ValueAttribution()
        {
            SymmetricMatrix sm = SymMat.Clone();
            Assert.AreEqual(2, sm.GetValueAt(1, 0), 1E-12);
            Assert.AreEqual(5, sm.GetValueAt(2, 1), 1E-12);

            sm.SetValueAt(2, 1, 15);
            Assert.AreEqual(15, sm.GetValueAt(1, 2), 1E-12);
            //		System.out.println(sm.toString());
        }

        [TestMethod]
        public void Test03MatrixMultiplicationByItself()
        {
            SymmetricMatrix sm = SymMat.Clone();

            //		System.out.println("Matrix sm = " + sm.toString());
            Matrix smPow2 = sm.Multiply(sm);

            Matrix m = new Matrix(3, 3);
            m.SetValueAt(0, 0, 1);
            m.SetValueAt(0, 1, 2);
            m.SetValueAt(0, 2, 3);
            m.SetValueAt(1, 0, 2);
            m.SetValueAt(1, 1, 4);
            m.SetValueAt(1, 2, 5);
            m.SetValueAt(2, 0, 3);
            m.SetValueAt(2, 1, 5);
            m.SetValueAt(2, 2, 6);
            //		System.out.println("Matrix m = " + m.toString());

            Matrix mPow2 = m.Multiply(m);
            Assert.IsTrue(smPow2 is SymmetricMatrix);
            Assert.IsTrue(!(mPow2 is SymmetricMatrix));
            Assert.IsTrue(smPow2.Equals(mPow2));
        }


        [TestMethod]
        public void Test04KroneckerProductOfSymmetricMatrices()
        {
            SymmetricMatrix sm = new(2);
            sm.SetValueAt(0, 0, 10);
            sm.SetValueAt(0, 1, 20);
            sm.SetValueAt(1, 1, 40);

            Matrix m = new(2, 2);
            m.SetValueAt(0, 0, 10);
            m.SetValueAt(0, 1, 20);
            m.SetValueAt(1, 0, 20);
            m.SetValueAt(1, 1, 40);

            Matrix kProd = SymMat.GetKroneckerProduct(m);
            Assert.IsTrue(kProd.IsSymmetric());

            Matrix kProd2 = SymMat.GetKroneckerProduct(sm);
            Assert.IsTrue(kProd2 is SymmetricMatrix);

            Assert.IsTrue(kProd2.Equals(kProd));
        }


        [TestMethod]
        public void Test05InverseMatrix()
        {
            Matrix sm = SymMat.Clone();
            Matrix invSM = sm.GetInverseMatrix();
            Matrix smTimesInvSM = sm.Multiply(invSM);

            Assert.IsTrue(invSM is SymmetricMatrix);

            Assert.IsTrue(smTimesInvSM.Equals(Matrix.GetIdentityMatrix(sm.m_iRows)));
        }

        [TestMethod]
        public void Test06InverseMatrixWithMultiplyBlocks()
        {
            Matrix sm = SymMat.MatrixDiagBlock(SymMat);
            Assert.IsTrue(sm is SymmetricMatrix);

            Matrix invSM = sm.GetInverseMatrix();
            Matrix smTimesInvSM = sm.Multiply(invSM);

            Assert.IsTrue(invSM is SymmetricMatrix);

            Assert.IsTrue(smTimesInvSM.Equals(Matrix.GetIdentityMatrix(sm.m_iRows)));
        }

        [TestMethod]
        public void Test07IsserlisMatrix()
        {
            Matrix m = new(3, 3);
            m.SetValueAt(0, 0, 1);
            m.SetValueAt(0, 1, 2);
            m.SetValueAt(0, 2, 3);
            m.SetValueAt(1, 0, 2);
            m.SetValueAt(1, 1, 4);
            m.SetValueAt(1, 2, 5);
            m.SetValueAt(2, 0, 3);
            m.SetValueAt(2, 1, 5);
            m.SetValueAt(2, 2, 6);

            Assert.IsTrue(m.IsSymmetric());

            Matrix isserlis = m.GetIsserlisMatrixOnlyForTestPurpose();
            //		System.out.println(isserlis);
            Assert.IsTrue(isserlis.IsSymmetric());

            Matrix isserlis2 = SymMat.GetIsserlisMatrix();
            Assert.IsTrue(isserlis2 is SymmetricMatrix);

            Assert.IsTrue(isserlis.Equals(isserlis2));

        }

        [TestMethod]
        public void Test08SymSquare_SquareSym()
        {
            Matrix symSquareVector = SymMat.SymSquare();
            Assert.AreEqual(6, symSquareVector.GetNumberOfElements());
            SymmetricMatrix originalMatrix = symSquareVector.SquareSym();
            Assert.IsTrue(SymMat.Equals(originalMatrix));
        }

    }

}
