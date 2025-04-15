using REpiceaLight.math;
using REpiceaLight.stats.estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.stats.estimates
{
    [TestClass]
    public sealed class MonteCarloEstimateTest
    {

        private static int NbRealizations = 50000;

        [TestMethod]
        public void Test01Percentiles()
        {
            MonteCarloEstimate estimate = new();
            Random random = new();
            Matrix m;
            for (int i = 0; i < NbRealizations; i++)
            {
                m = new Matrix(2, 1);
                m.SetValueAt(0, 0, random.NextDouble());
                m.SetValueAt(1, 0, random.NextDouble() * 2);
                estimate.AddRealization(m);
            }

            Matrix percentiles = estimate.GetQuantileForProbability(0.95);
            Assert.AreEqual(0.95, percentiles.GetValueAt(0, 0), 1E-2);
            Assert.AreEqual(0.95 * 2, percentiles.GetValueAt(1, 0), 1E-2);
        }

        //[TestMethod]
        //public void Test02UnivariateTotalVarianceLaw()
        //{
        //    double outerStd = 5d;
        //    double innerStd = 5d;
        //    int sampleSize = 100;
        //    Random generator = new();
        //    LawOfTotalVarianceMonteCarloEstimate output = new LawOfTotalVarianceMonteCarloEstimate();
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        double meanForThisRealization = generator.nextGaussian() * outerStd;
        //        PopulationMeanEstimate estimate = new PopulationMeanEstimate();
        //        Matrix obs;
        //        for (int j = 0; j < sampleSize; j++)
        //        {
        //            obs = new Matrix(1, 1);
        //            obs.setValueAt(0, 0, generator.nextGaussian() * innerStd + meanForThisRealization);
        //            estimate.addObservation(obs, j + "");
        //        }
        //        output.addRealization(estimate);
        //    }
        //    double totalVariance = output.getVariance().getValueAt(0, 0);
        //    double expectedVariance = outerStd * outerStd + innerStd * innerStd / sampleSize;
        //    double relativeDifference = (totalVariance - expectedVariance) / expectedVariance;
        //    System.out.println("Relative difference of " + relativeDifference);
        //    Assert.assertEquals(0d, relativeDifference, 0.4);
        //}

    }
}
