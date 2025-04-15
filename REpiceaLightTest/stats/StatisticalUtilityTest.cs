using REpiceaLight.math;
using REpiceaLight.stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.StatisticalUtility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLightTest.stats
{
    [TestClass]
    public sealed class StatisticalUtilityTest
    {

        [TestMethod]
        public void Test01SimpleCombinationCounts()
        {

            long actual = StatisticalUtility.GetCombinations(5, 5);
            Assert.AreEqual(1, actual);

            actual = StatisticalUtility.GetCombinations(10, 8);
            Assert.AreEqual(45, actual);

            actual = StatisticalUtility.GetCombinations(10, 2);
            Assert.AreEqual(45, actual);

        }

        [TestMethod]
        public void Test02SimpleAR1Matrix()
        {
            Matrix ar1Matrix2 = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, 0.95 }, TypeMatrixR.POWER, new Matrix(10, 1, 1, 1));
            double expected = Math.Pow(0.95, 9);
            double actual = ar1Matrix2.GetValueAt(9, 0);
            Assert.AreEqual(expected, actual, 1E-8);
        }

        [TestMethod]
        public void Test03RapidAR1MatrixInversion()
        {
            Matrix ar1Matrix2 = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, 0.95 }, TypeMatrixR.POWER, new Matrix(10, 1, 1, 1));

            Matrix invMatrix = StatisticalUtility.GetInverseCorrelationAR1Matrix(ar1Matrix2.m_iRows, 0.95);
            Matrix originalInvMatrix = ar1Matrix2.GetInverseMatrix();
            Matrix diff = invMatrix.Subtract(originalInvMatrix).GetAbsoluteValue();
            bool isDifferent = diff.AnyElementLargerThan(1E-8);
            Assert.IsTrue(!isDifferent);
        }

        [TestMethod]
        public void Test04InversionCovarianceMatrixBasedOnAR1MatrixInversion()
        {
            Matrix diag = new Matrix(10, 1, 0.5, 0.25).MatrixDiagonal();
            Matrix ar1Matrix2 = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, 0.95 }, TypeMatrixR.POWER, new Matrix(10, 1, 1, 1));

            Matrix completeMatrix = diag.Multiply(ar1Matrix2).Multiply(diag);
            Matrix originalInvMatrix = completeMatrix.GetInverseMatrix();

            Matrix invCorrMatrix = StatisticalUtility.GetInverseCorrelationAR1Matrix(ar1Matrix2.m_iRows, 0.95);
            Matrix invDiag = new Matrix(10, 1, 0.5, 0.25).ElementWisePower(-1d).MatrixDiagonal();
            Matrix invMatrix = invDiag.Multiply(invCorrMatrix).Multiply(invDiag);
            Matrix diff = invMatrix.Subtract(originalInvMatrix).GetAbsoluteValue();
            bool isDifferent = diff.AnyElementLargerThan(1E-8);
            Assert.IsTrue(!isDifferent);
        }

        [TestMethod]
        public void Test05InversionCovarianceMatrixBasedOnAR1MatrixInversion2()
        {
            Matrix diag = new Matrix(10, 1, 0.5, 0.25).MatrixDiagonal();
            Matrix ar1Matrix2 = StatisticalUtility.ConstructRMatrix(new List<double> { 1d, 0.95 }, TypeMatrixR.POWER, new Matrix(10, 1, 1, 1));

            Matrix completeMatrix = diag.Multiply(ar1Matrix2).Multiply(diag);
            Matrix originalInvMatrix = completeMatrix.GetInverseMatrix();

            Matrix invCorrMatrix = StatisticalUtility.GetInverseCorrelationAR1Matrix(ar1Matrix2.m_iRows, 0.95);
            Matrix std = diag.DiagonalVector();
            Matrix invDiag = std.Multiply(std.Transpose()).ElementWisePower(-1d);
            Matrix invMatrix = invDiag.ElementWiseMultiply(invCorrMatrix);
            Matrix diff = invMatrix.Subtract(originalInvMatrix).GetAbsoluteValue();
            bool isDifferent = diff.AnyElementLargerThan(1E-8);
            Assert.IsTrue(!isDifferent);
        }



        [TestMethod]
        public void Test06DimensionEuclideanDistance()
        {
            List<double> covParms = new List<double> { 1d, 0.85 };
            Matrix coordinateX = new Matrix(2, 1, 0, 1);
            Matrix coordinateY = new Matrix(2, 1, 1, -1);
            Matrix matR = StatisticalUtility.ConstructRMatrix(covParms, TypeMatrixR.POWER, new Matrix[] { coordinateX, coordinateY });
            double expected = Math.Pow(covParms[1], Math.Sqrt(2d));
            double actual = matR.GetValueAt(0, 1);
            Assert.AreEqual(expected, actual, 1E-8);
        }

        [TestMethod]
        public void Test07SimpleComparisonBetweenExponentialAndPowerCovarianceStructure()
        {
            List<double> covParmsExponential = new List<double> { 1d, 1.00 };
            double conversionToPower = Math.Exp(-1d / covParmsExponential[1]);
            List<double> covParmsPower = new List<double> { 1d, conversionToPower };

            Matrix coordinateX = new Matrix(2, 1, 0, 1);
            Matrix coordinateY = new Matrix(2, 1, 1, -1);
            Matrix matRPower = StatisticalUtility.ConstructRMatrix(covParmsPower, TypeMatrixR.POWER, new Matrix[] { coordinateX, coordinateY });
            Matrix matRExp = StatisticalUtility.ConstructRMatrix(covParmsExponential, TypeMatrixR.EXPONENTIAL, new Matrix[] { coordinateX, coordinateY });
            double expectedPow = Math.Pow(covParmsPower[1], Math.Sqrt(2d));
            double actualPow = matRPower.GetValueAt(0, 1);
            Assert.AreEqual(expectedPow, actualPow, 1E-8);

            double expectedExp = Math.Exp(-Math.Sqrt(2d) / covParmsExponential[1]);
            double actualExp = matRPower.GetValueAt(0, 1);
            Assert.AreEqual(expectedExp, actualExp, 1E-8);

            bool areEqual = !matRPower.Subtract(matRExp).GetAbsoluteValue().AnyElementLargerThan(1E-8);
            Assert.IsTrue(areEqual);
        }



    }
}
