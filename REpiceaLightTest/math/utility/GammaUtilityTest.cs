using REpiceaLight.math.utility;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLightTest.math.utility
{
    [TestClass]
    public sealed class GammaUtilityTest
    {

      //  [TestMethod]
      //  public void Test01Values()
      //  {
      //      String filename = ObjectUtility.getPackagePath(getClass()) + "gammaTest.csv";
    		//CSVReader reader = new CSVReader(filename);
      //      Object[] record;
      //      int i = 0;
	    	//while ((record = reader.nextRecord()) != null) {
		    //	double x = Double.parseDouble(record[1].ToString());
      //          double expectedValue = Double.parseDouble(record[2].ToString());
      //          double actualValue = GammaUtility.Gamma(x);
      //          Assert.AreEqual(expectedValue, actualValue, 1E-8);
    		//	i++;
	    	//}
      //      reader.close();
		    //Console.WriteLine("GammaUtility successfully tested on " + i + " observations");
      //  }


        [TestMethod]
        public void Test02InverseGammaFunction()
        {
            for (double d = 2; d < 15; d += 0.5)
            {
                double gammaValue = GammaUtility.Gamma(d);
                double actual = GammaUtility.InverseGamma(gammaValue);
                Console.WriteLine("Expected = " + d + "; Gamma value = " + gammaValue + "; Actual = " + actual);
                double tolerance = 1E-2;
                if (d == 2)
                {
                    tolerance = 2.5E-2;
                }
                Assert.AreEqual(d, actual, tolerance);
            }
        }

        [TestMethod]
        public void Test03DigammaImplementation()
        {
            double observed = GammaUtility.Digamma(0.5);
            double expected = -1.96351002602142;
            Assert.AreEqual(expected, observed, 1E-12);
            observed = GammaUtility.Digamma(5);
            expected = 1.5061176684318;
            Assert.AreEqual(expected, observed, 1E-12);
        }

        [TestMethod]
        public void Test04TrigammaImplementation()
        {
            double observed = GammaUtility.Trigamma(0.5);
            double expected = 4.93480220054468;
            Assert.AreEqual(expected, observed, 1E-12);
            observed = GammaUtility.Trigamma(5);
            expected = 0.221322955737115;
            Assert.AreEqual(expected, observed, 1E-12);
        }

    }

}
