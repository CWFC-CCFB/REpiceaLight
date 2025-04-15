using REpiceaLight.math.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.math.utility
{

    [TestClass]
    public sealed class MathUtilityTest
    {

        [TestMethod]
        public void Test01SimpleFactorials()
        {
            double actual = MathUtility.Factorial(5);
            Assert.AreEqual(120, actual, 1E-8);

            actual = MathUtility.Factorial(0);
            Assert.AreEqual(1, actual, 1E-8);

            actual = MathUtility.Factorial(10);
            Assert.AreEqual(3628800, actual, 1E-8);
        }


        [TestMethod]
        public void Test02FactorialRatios()
        {
            double actual = MathUtility.FactorialRatio(5, 3);
            Assert.AreEqual(20, actual, 1E-8);

            actual = MathUtility.FactorialRatio(10, 7);
            Assert.AreEqual(720, actual, 1E-8);

            actual = MathUtility.FactorialRatio(1, 0);
            Assert.AreEqual(1, actual, 1E-8);
        }

    }


}
