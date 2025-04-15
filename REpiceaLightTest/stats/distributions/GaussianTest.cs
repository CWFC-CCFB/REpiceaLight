using REpiceaLight.math.utility;
using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.stats.distributions
{
    [TestClass]
    public sealed class GaussianTest
    {

        [TestMethod]
        public void Test01BivariateCumulativeProbabilities()
        {
            double x1 = -0.2876339;
            double x2 = 1.125041;
            double rho = -0.6018568;
            double biv11 = GaussianUtility.GetBivariateCumulativeProbability(x1, x2, false, false, rho);
            Assert.AreEqual(0.2832904938411274, biv11, 1E-10);

            double biv01 = GaussianUtility.GetBivariateCumulativeProbability(x1, x2, true, false, rho);
            Assert.AreEqual(0.5864236757641986, biv01, 1E-10);

            double biv10 = GaussianUtility.GetBivariateCumulativeProbability(x1, x2, false, true, rho);
            Assert.AreEqual(0.10352300242645603, biv10, 1E-10);

            double biv00 = GaussianUtility.GetBivariateCumulativeProbability(x1, x2, true, true, rho);
            Assert.AreEqual(0.026762827968218002, biv00, 1E-10);
        }

        [TestMethod]
        public void Test02Quantile()
        {
            for (int i = 1; i < 20; i++)
            {
                double expectedCDFValue = i * .05;
                double quantile = GaussianUtility.GetQuantile(expectedCDFValue);
                double cdfValue = GaussianUtility.GetCumulativeProbability(quantile);
                Assert.AreEqual(expectedCDFValue, cdfValue, 1E-9);
            }
        }

        [TestMethod]
        public void Test03ProbabilityDensity()
        {
            double prob = GaussianUtility.GetProbabilityDensity(10, 5, 9);
            Assert.AreEqual(0.03315905, prob, 1E-8);
            prob = GaussianUtility.GetProbabilityDensity(-6, -1, 100);
            Assert.AreEqual(0.03520653, prob, 1E-8);
            try
            {
                prob = GaussianUtility.GetProbabilityDensity(-6, -1, -100);
                Assert.Fail();  // if get to this line, means that the exception was not throw.
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void Test04Densities()
        {
            double w = 2;
            double mu = 1.2;
            double variance = 2;
            double std = Math.Sqrt(variance);
            double dens1 = GaussianUtility.GetProbabilityDensity((w - mu) / std) / Math.Sqrt(variance);
            double dens2 = GaussianUtility.GetProbabilityDensity(w, mu, variance);
            Assert.AreEqual(dens2, dens1, 1E-8);
        }

    }

}
