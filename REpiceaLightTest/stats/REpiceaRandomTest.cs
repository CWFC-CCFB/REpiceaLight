using REpiceaLight.math;
using REpiceaLight.stats;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.stats
{
    [TestClass]
    public sealed class REpiceaRandomTest
    {

        [TestMethod]
        public void Test01GammaMean()
        {
            double dDispersionGamma = 0.61766452969979;
            double fGammaMean = 1.813311388045523;
            double shape = dDispersionGamma;
            double scale = fGammaMean / dDispersionGamma;
            REpiceaRandom randomGenerator = new();

            double mean = 0;
            int maxIter = 500000;
            double meanFactor = 1d / maxIter;
            for (int i = 0; i < maxIter; i++)
            {
                double randomDeviate;
                try
                {
                    randomDeviate = randomGenerator.NextGamma(shape, scale);
                    mean += randomDeviate * meanFactor;
                }
                catch (Exception )
                {
                }

            }

            Console.WriteLine("Simulated mean = " + mean + "; Expected mean = " + fGammaMean);
            Assert.AreEqual(fGammaMean, mean, 1E-2);
        }


        [TestMethod]
        public void Test02GammaVariance()
        {
            double dDispersionGamma = 0.61766452969979;
            double fGammaMean = 1.813311388045523;
            //		double scale = dDispersionGamma;
            //		double shape = fGammaMean / dDispersionGamma;
            double shape = dDispersionGamma;
            double scale = fGammaMean / dDispersionGamma;
            REpiceaRandom randomGenerator = new REpiceaRandom();
            //		RandomDataImpl randomGenerator = new RandomDataImpl();
            double variance = fGammaMean * fGammaMean / dDispersionGamma;

            int maxIter = 500000;
            MonteCarloEstimate estimate = new();
            Matrix realization;
            for (int i = 0; i < maxIter; i++)
            {
                realization = new Matrix(1, 1);
                realization.SetValueAt(0, 0, randomGenerator.NextGamma(shape, scale));
                estimate.AddRealization(realization);
            }
            double actual = estimate.GetVariance().GetValueAt(0, 0);
            double mean = estimate.GetMean().GetValueAt(0, 0);
            Console.WriteLine("Simulated variance = " + actual + "; Expected variance = " + variance);
            Assert.AreEqual(variance, actual, 0.1);
        }

        [TestMethod]
        public void Test03NegativeBinomialMean()
        {
            double dispersion = 1.41209128903651;
            double expectedMean = 0.5172784824575968;
            REpiceaRandom randomGenerator = new();

            double mean = 0;
            int maxIter = 500000;
            double meanFactor = 1d / maxIter;
            for (int i = 0; i < maxIter; i++)
            {
                double randomDeviate = 0;
                try
                {
                    randomDeviate = randomGenerator.NextNegativeBinomial(expectedMean, dispersion);
                    mean += randomDeviate * meanFactor;
                }
                catch (Exception )
                {
                }

            }

            Console.WriteLine("Simulated mean = " + mean + "; Expected mean = " + expectedMean);
            Assert.AreEqual(expectedMean, mean, 1E-2);
        }


        [TestMethod]
        public void Test04NegativeBinomialVariance()
        {
            double dispersion = 1.41209128903651;
            double expectedMean = 0.5172784824575968;
            REpiceaRandom randomGenerator = new();
            double expectedVariance = expectedMean + dispersion * expectedMean * expectedMean;
            int maxIter = 500000;
            MonteCarloEstimate estimate = new();
            Matrix realization;
            for (int i = 0; i < maxIter; i++)
            {
                realization = new(1, 1);
                realization.SetValueAt(0, 0, randomGenerator.NextNegativeBinomial(expectedMean, dispersion));
                estimate.AddRealization(realization);
            }
            double actual = estimate.GetVariance().GetValueAt(0, 0);

            double mean = estimate.GetMean().GetValueAt(0, 0);
            Console.WriteLine("Simulated variance = " + actual + "; Expected variance = " + expectedVariance);
            Assert.AreEqual(expectedVariance, actual, 1E-1);
        }

        [TestMethod]
        public void Test05BetaMeanAndVariance()
        {
            double scale1 = 1d;
            double scale2 = 2d;
            double expectedMean = scale1 / (scale1 + scale2);
            double expectedVariance = scale1 * scale2 / ((scale1 + scale2) * (scale1 + scale2) * (scale1 + scale2 + 1));
            REpiceaRandom randomGenerator = new();
            int maxIter = 500000;
            MonteCarloEstimate estimate = new();
            Matrix realization;
            for (int i = 0; i < maxIter; i++)
            {
                realization = new Matrix(1, 1);
                realization.SetValueAt(0, 0, randomGenerator.NextBeta(scale1, scale2));
                estimate.AddRealization(realization);
            }
            double actualMean = estimate.GetMean().GetValueAt(0, 0);
            double actualVariance = estimate.GetVariance().GetValueAt(0, 0);
            Console.WriteLine("Simulated mean = " + actualMean + "; Expected variance = " + expectedMean);
            Assert.AreEqual(expectedMean, actualMean, 5E-3);
            Console.WriteLine("Simulated variance = " + actualVariance + "; Expected variance = " + expectedVariance);
            Assert.AreEqual(expectedVariance, actualVariance, 1E-3);
        }


        [TestMethod]
        public void Test06ChiSquareMeanAndVariance()
        {
            int upsilon = 100;
            double expectedMean = upsilon;
            double expectedVariance = 2 * upsilon;
            REpiceaRandom randomGenerator = new();
            int maxIter = 500000;
            MonteCarloEstimate estimate = new();
            Matrix realization;
            long initial = DateTime.Now.Ticks  / TimeSpan.TicksPerMillisecond;
            for (int i = 0; i < maxIter; i++)
            {
                realization = new(1, 1);
                realization.SetValueAt(0, 0, randomGenerator.NextChiSquare(upsilon));
                estimate.AddRealization(realization);
            }
            long finalTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - initial; 
            Console.WriteLine("Time to compute deviates = " + finalTime + " ms.");

            double actualMean = estimate.GetMean().GetValueAt(0, 0);
            double actualVariance = estimate.GetVariance().GetValueAt(0, 0);
            Console.WriteLine("Simulated mean = " + actualMean + "; Expected variance = " + expectedMean);
            Assert.AreEqual(expectedMean, actualMean, 7E-2);
            Console.WriteLine("Simulated variance = " + actualVariance + "; Expected variance = " + expectedVariance);
            Assert.AreEqual(expectedVariance, actualVariance, 2);
        }

    }

}
