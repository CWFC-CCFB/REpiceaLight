using REpiceaLight.math.utility;
using REpiceaLight.math;
using REpiceaLight.stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.math.utility
{
    [TestClass]
    public sealed class NegativeBinomialUtilityTest
    {

        [TestMethod]
        public void Test01SimpleValue()
        {
            double observed = NegativeBinomialUtility.GetMassProbability(2, 1, .5);
            double expected = 0.14814814814814831;
            Assert.AreEqual(expected, observed, 1E-8);
        }

        [TestMethod]
        public void Test02Quantile()
        {
            int observed = NegativeBinomialUtility.GetQuantile(0.5, 1, .8);
            Assert.AreEqual(1, observed);
        }

        //[TestMethod]
        //public void Test03Mean()
        //{
        //    double mu = 1d;
        //    double theta = .8;
        //    MonteCarloEstimate est = new();
        //    int nbRealizations = 50000;
        //    for (int i = 0; i < nbRealizations; i++)
        //    {
        //        int observed = NegativeBinomialUtility.GetQuantile(StatisticalUtility.GetRandom().NextDouble(),
        //                mu,
        //                theta);
        //        est.addRealization(new Matrix(1, 1, observed, 0));
        //    }
        //    double mean = est.getMean().getValueAt(0, 0);
        //    double variance = est.getVariance().getValueAt(0, 0);
        //    Console.WriteLine("Expected mean = " + mu + "; Actual mean = " + mean);
        //    Assert.AreEqual(mu, mean, 2.5E-2);
        //    double expectedVariance = mu + theta * mu * mu;
        //    Console.WriteLine("Expected variance = " + expectedVariance + "; Actual variance = " + variance);
        //    Assert.AreEqual(expectedVariance, variance, 1E-1);
        //}


        //[TestMethod]
        //public void Test04Mean()
        //{
        //    double mu = 1.5;
        //    double theta = .5;
        //    MonteCarloEstimate est = new();
        //    int nbRealizations = 50000;
        //    for (int i = 0; i < nbRealizations; i++)
        //    {
        //        int observed = NegativeBinomialUtility.GetQuantile(StatisticalUtility.GetRandom().NextDouble(),
        //                mu,
        //                theta);
        //        est.addRealization(new Matrix(1, 1, observed, 0));
        //    }
        //    double mean = est.getMean().getValueAt(0, 0);
        //    double variance = est.getVariance().getValueAt(0, 0);
        //    Assert.AreEqual(mu, mean, 3E-2);
        //    Assert.AreEqual(mu + theta * mu * mu, variance, 1E-1);
        //}

    }
}
