using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.math
{
    [TestClass]
    public sealed class DiagonalMatrixTest
    {

        static DiagonalMatrix DiagMat;

        [TestInitialize]
        public void Initialize()
        {
            DiagonalMatrix sm = new(3);
            sm.SetValueAt(0, 0, 1);
            sm.SetValueAt(1, 1, 4);
            sm.SetValueAt(2, 2, 6);
            DiagMat = sm;
        }


        [TestMethod]
        public void Test01InternalArrayConstruction()
        {
            DiagonalMatrix sm = new (4);
            Assert.AreEqual(1, sm.m_afData.Length);
            Assert.AreEqual(4, sm.m_afData[0].Length);
        }

        [TestMethod]
        public void Test02ValueAttribution()
        {
            SymmetricMatrix sm = DiagMat.Clone();
            Assert.AreEqual(4, sm.GetValueAt(1, 1), 1E-12);
            Assert.AreEqual(0, sm.GetValueAt(2, 1), 1E-12);

            sm.SetValueAt(2, 2, 15);
            Assert.AreEqual(15, sm.GetValueAt(2, 2), 1E-12);
            try
            {
                sm.SetValueAt(0, 1, 0);
                Assert.Fail("Should have thrown an exception!");
            }
            catch (Exception e) { }
        }

        [TestMethod]
        public void Test03MatrixMultiplicationByItself()
        {
            DiagonalMatrix sm = DiagMat.Clone();

            Matrix smPow2 = sm.Multiply(sm);

            Assert.IsTrue(smPow2 is DiagonalMatrix);
            Assert.AreEqual(sm.GetValueAt(1, 1) * sm.GetValueAt(1, 1), smPow2.GetValueAt(1, 1), 1E-8);
        }


        [TestMethod]
        public void Test04InverseMatrix()
        {
            DiagonalMatrix sm = DiagMat.Clone();
            DiagonalMatrix invSM = sm.GetInverseMatrix();
            Matrix smTimesInvSM = sm.Multiply(invSM);

            Assert.IsTrue(invSM is DiagonalMatrix);

            Assert.IsTrue(smTimesInvSM.Equals(Matrix.GetIdentityMatrix(sm.m_iRows)));
        }


    }

}
