using REpiceaLight.math;
using REpiceaLight.stats.distributions;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.stats.distributions
{
    [TestClass]
    public sealed class DistributionTest
    {

        /*
         * This test uses a Multivariate Gaussian distribution as a reference and compute a Monte Carlo estimator. The mean and the variances are then compared.
         */
        [TestMethod]
        public void Test01StochasticSimulationFromGaussianDistribution()
        {
            int nbReal = 1000000;
            EmpiricalDistribution npDist = new();
            Matrix mean = new(2, 1);
            mean.SetValueAt(0, 0, 2d);
            mean.SetValueAt(1, 0, 3d);
            SymmetricMatrix variance = new(2);
            variance.SetValueAt(0, 0, 0.5);
            variance.SetValueAt(1, 0, 0.4);
            variance.SetValueAt(1, 1, 1d);

            GaussianEstimate estimate = new(mean, variance);

            for (int i = 0; i < nbReal; i++)
                npDist.AddRealization(estimate.GetRandomDeviate());

            Matrix simulatedMean = npDist.GetMean();
            Matrix res = simulatedMean.Subtract(mean);
            double sse = res.Transpose().Multiply(res).GetValueAt(0, 0);
            Console.WriteLine("Squared difference of the means = " + sse);

            Assert.AreEqual(0d, sse, 1E-4);

            SymmetricMatrix simulatedVariances = npDist.GetVariance();
            Matrix diff = simulatedVariances.Subtract(variance);
            sse = diff.ElementWiseMultiply(diff).GetSumOfElements();
            Console.WriteLine("Squared difference of the variances = " + sse);

            Assert.AreEqual(0d, sse, 1E-4);

        }

        //    /*
        //     * This test uses a univariate truncated Gaussian distribution as a reference and compute a Monte Carlo estimator. The mean and the variances are then compared.
        //     */
        //    @Test
        //    public void stochasticSimulationFromTruncatedStandardGaussianDistribution()
        //    {
        //        int nbReal = 1000000;

        //        TruncatedGaussianDistribution distribution = new TruncatedGaussianDistribution();
        //        distribution.setUpperBoundValue(new Matrix(1, 1));
        //        MonteCarloEstimate estimate = new MonteCarloEstimate();

        //        for (int i = 0; i < nbReal; i++)
        //        {
        //            estimate.addRealization(distribution.getRandomRealization());
        //        }

        //        Matrix simulatedMean = estimate.getMean();
        //        Matrix simulatedVariance = estimate.getVariance();
        //        Matrix expectedMean = distribution.getMean();
        //        Matrix expectedVariance = distribution.getVariance();
        //        Assert.assertEquals("Testing the means", expectedMean.getValueAt(0, 0), simulatedMean.getValueAt(0, 0), 2E-3);
        //        Assert.assertEquals("Testing the variances", expectedVariance.getValueAt(0, 0), simulatedVariance.getValueAt(0, 0), 3E-3);
        //    }


        //    /*
        //     * This test uses a univariate truncated Gaussian distribution as a reference and compute a Monte Carlo estimator. The mean and the variances are then compared.
        //     */
        //    @Test
        //public void stochasticSimulationFromTruncatedGaussianDistribution()
        //    {
        //        int nbReal = 1000000;

        //        TruncatedGaussianDistribution distribution = new TruncatedGaussianDistribution(10, 20);
        //        Matrix upperBound = new Matrix(1, 1);
        //        upperBound.setValueAt(0, 0, 12);
        //        distribution.setUpperBoundValue(upperBound);

        //        MonteCarloEstimate estimate = new MonteCarloEstimate();

        //        for (int i = 0; i < nbReal; i++)
        //        {
        //            estimate.addRealization(distribution.getRandomRealization());
        //        }

        //        double simulatedMean = estimate.getMean().getValueAt(0, 0);
        //        double simulatedVariance = estimate.getVariance().getValueAt(0, 0);
        //        double expectedMean = distribution.getMean().getValueAt(0, 0);
        //        double expectedVariance = distribution.getVariance().getValueAt(0, 0);
        //        System.out.println("Expected mean = " + expectedMean + ", actual mean = " + simulatedMean);
        //        Assert.assertEquals("Testing the means", expectedMean, simulatedMean, 1.5E-2);
        //        System.out.println("Expected variance = " + expectedVariance + ", actual variance = " + simulatedVariance);
        //        Assert.assertEquals("Testing the variances", expectedVariance, simulatedVariance, 5E-2);
        //    }

    }

}
