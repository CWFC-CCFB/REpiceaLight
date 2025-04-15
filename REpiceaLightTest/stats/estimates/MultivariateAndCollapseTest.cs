using REpiceaLight.math;
using REpiceaLight.stats.estimates;
using REpiceaLight.stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static REpiceaLight.stats.IDistribution;
using System.Collections.Specialized;

namespace REpiceaLightTest.stats.estimates
{
    [TestClass]
    public sealed class MultivariateAndCollapseTest
    {

        [TestMethod]
        public void Test01CompleteCollapsingWithMonteCarloEstimate()
        {
            int nElements = 10;
            MonteCarloEstimate est = new();
            for (int i = 0; i < 1000; i++)
                est.AddRealization(StatisticalUtility.DrawRandomVector(nElements, DistributionType.GAUSSIAN));

            Matrix basicMean = est.GetMean();
            Assert.IsTrue(basicMean.m_iRows == nElements);
            SymmetricMatrix basicVariance = est.GetVariance();
            Assert.IsTrue(basicVariance.m_iRows == nElements);
            Assert.IsTrue(basicVariance.m_iCols == nElements);

            List<string> rowIndex = new();
            for (int i = 0; i < nElements; i++)
                rowIndex.Add("" + i);

            est.SetRowIndex(rowIndex);
            OrderedDictionary collapseIndices = new();
            collapseIndices["all"] = rowIndex;
            IEstimate collapsedEstimate = est.CollapseEstimate(collapseIndices);

            Matrix collapsedMean = collapsedEstimate.GetMean();
            Assert.IsTrue(collapsedMean.m_iRows == 1);
            Assert.AreEqual(basicMean.GetSumOfElements(), collapsedMean.GetValueAt(0, 0), 1E-8);
            SymmetricMatrix collapsedVariance = collapsedEstimate.GetVariance();
            Assert.IsTrue(collapsedVariance.m_iRows == 1);
            Assert.IsTrue(collapsedVariance.m_iCols == 1);
            Assert.AreEqual(basicVariance.GetSumOfElements(), collapsedVariance.GetValueAt(0, 0), 1E-8);
        }

        [TestMethod]
        public void Test02PartialCollapsingWithMonteCarloEstimate()
        {
            int nElements = 10;
            MonteCarloEstimate est = new();
            for (int i = 0; i < 1000; i++)
                est.AddRealization(StatisticalUtility.DrawRandomVector(nElements, DistributionType.GAUSSIAN));

            Matrix basicMean = est.GetMean();
            Assert.IsTrue(basicMean.m_iRows == nElements);
            SymmetricMatrix basicVariance = est.GetVariance();
            Assert.IsTrue(basicVariance.m_iRows == nElements);
            Assert.IsTrue(basicVariance.m_iCols == nElements);

            List<string> rowIndex = new();
            for (int i = 0; i < nElements; i++)
                rowIndex.Add("" + i);

            est.SetRowIndex(rowIndex);
            OrderedDictionary collapseIndices = new();
            collapseIndices["group1"] = new List<string>();
            collapseIndices["group2"] = new List<string>();
            for (int i = 0; i < rowIndex.Count; i++)
            {
                if (i < 3)
                    ((List<string>)collapseIndices["group1"]).Add(rowIndex[i]);
                else
                    ((List<string>)collapseIndices["group2"]).Add(rowIndex[i]);
            }

            IEstimate collapsedEstimate = est.CollapseEstimate(collapseIndices);

            Matrix collapsedMean = collapsedEstimate.GetMean();
            Assert.IsTrue(collapsedMean.m_iRows == 2);
            Assert.AreEqual(basicMean.GetSumOfElements(), collapsedMean.GetSumOfElements(), 1E-8);
            SymmetricMatrix collapsedVariance = collapsedEstimate.GetVariance();
            Assert.IsTrue(collapsedVariance.m_iRows == 2);
            Assert.IsTrue(collapsedVariance.m_iCols == 2);
            Assert.AreEqual(basicVariance.GetSumOfElements(), collapsedVariance.GetSumOfElements(), 1E-8);
        }

    }

}
