using REpiceaLight.math;
using REpiceaLight.stats;
using System.Formats.Asn1;
using System.Xml.Serialization;
using static REpiceaLight.stats.StatisticalUtility;

namespace REpiceaLightTest.math
{
    [TestClass]
    public sealed class MatrixTest
    {

        [TestMethod]
        public void Test01Multiplication()
        {
            Matrix m1 = new(2, 2, 1, 1);
            Matrix m2 = new(2, 2, 2, 1);
            Matrix product = m1.Multiply(m2);
            Assert.AreEqual(1 * 2 + 2 * 4, product.GetValueAt(0, 0), 1E-8);
            Assert.AreEqual(1 * 3 + 2 * 5, product.GetValueAt(0, 1), 1E-8);
            Assert.AreEqual(3 * 2 + 4 * 4, product.GetValueAt(1, 0), 1E-8);
            Assert.AreEqual(3 * 3 + 4 * 5, product.GetValueAt(1, 1), 1E-8);
        }

        [TestMethod]
        public void Test02BlockedInversedMatrix()
        {

            Matrix mat = new(9, 9);
            mat.SetValueAt(0, 0, 5.49);
            mat.SetValueAt(0, 4, 1.85);
            mat.SetValueAt(1, 1, 3.90);
            mat.SetValueAt(2, 2, 2.90);
            mat.SetValueAt(2, 3, 1.02);
            mat.SetValueAt(2, 5, 0.70);
            mat.SetValueAt(2, 6, 0.76);
            mat.SetValueAt(2, 7, 0.77);
            mat.SetValueAt(2, 8, 0.80);
            mat.SetValueAt(3, 3, 3.20);
            mat.SetValueAt(3, 5, 0.89);
            mat.SetValueAt(3, 6, 0.87);
            mat.SetValueAt(3, 7, 0.89);
            mat.SetValueAt(3, 8, 0.93);
            mat.SetValueAt(4, 4, 4.55);
            mat.SetValueAt(5, 5, 2.70);
            mat.SetValueAt(5, 6, 0.66);
            mat.SetValueAt(5, 7, 0.67);
            mat.SetValueAt(5, 8, 0.70);
            mat.SetValueAt(6, 6, 2.69);
            mat.SetValueAt(6, 7, 0.66);
            mat.SetValueAt(6, 8, 0.69);
            mat.SetValueAt(7, 7, 2.70);
            mat.SetValueAt(7, 8, 0.70);
            mat.SetValueAt(8, 8, 2.76);

            for (int i = 0; i < mat.m_iRows; i++)
                for (int j = i; j < mat.m_iCols; j++)
                    if (i != j)
                        mat.SetValueAt(j, i, mat.GetValueAt(i, j));

            Matrix invMat = mat.GetInverseMatrix();
            Matrix ident = mat.Multiply(invMat);
            Matrix diff = ident.Subtract(Matrix.GetIdentityMatrix(ident.m_iCols)).GetAbsoluteValue();
            bool equalToIdentity = !diff.AnyElementLargerThan(1E-15);
            Assert.IsTrue(equalToIdentity);
        }


        public void Test03toArrayTest()
        {

            Matrix mat = new(9, 9);
            mat.SetValueAt(0, 0, 5.49);
            mat.SetValueAt(0, 4, 1.85);
            mat.SetValueAt(1, 1, 3.90);
            mat.SetValueAt(2, 2, 2.90);
            mat.SetValueAt(2, 3, 1.02);
            mat.SetValueAt(2, 5, 0.70);
            mat.SetValueAt(2, 6, 0.76);
            mat.SetValueAt(2, 7, 0.77);
            mat.SetValueAt(2, 8, 0.80);
            mat.SetValueAt(3, 3, 3.20);
            mat.SetValueAt(3, 5, 0.89);
            mat.SetValueAt(3, 6, 0.87);
            mat.SetValueAt(3, 7, 0.89);
            mat.SetValueAt(3, 8, 0.93);
            mat.SetValueAt(4, 4, 4.55);
            mat.SetValueAt(5, 5, 2.70);
            mat.SetValueAt(5, 6, 0.66);
            mat.SetValueAt(5, 7, 0.67);
            mat.SetValueAt(5, 8, 0.70);
            mat.SetValueAt(6, 6, 2.69);
            mat.SetValueAt(6, 7, 0.66);
            mat.SetValueAt(6, 8, 0.69);
            mat.SetValueAt(7, 7, 2.70);
            mat.SetValueAt(7, 8, 0.70);
            mat.SetValueAt(8, 8, 2.76);

            double[][] array = mat.ToArray();
            Matrix mat2 = new(array);
            bool isThereAnyDiff = mat.Subtract(mat2).GetAbsoluteValue().AnyElementLargerThan(1E-8);
            Assert.IsTrue(!isThereAnyDiff);
        }

        [TestMethod]
        public void Test04InversionWithZeroCellsTest()
        {

            Matrix coordinates = new(20, 1, 0, 1);

            Matrix rMatrix = StatisticalUtility.ConstructRMatrix([2d, 0.2], TypeMatrixR.LINEAR, coordinates);
            Matrix invMat = rMatrix.GetInverseMatrix();

            Matrix ident = rMatrix.Multiply(invMat);

            Matrix diff = ident.Subtract(Matrix.GetIdentityMatrix(ident.m_iCols)).GetAbsoluteValue();

            bool equalToIdentity = !diff.AnyElementLargerThan(1E-10);

            Assert.IsTrue(equalToIdentity);
        }

        //public void Test05MemoryManagement()
        //{
        //    List<Matrix> myArrayList = new();
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        myArrayList.Add(new Matrix(700, 1, false)); // old implementation
        //    }
        //    double currentMemoryLoad = REpiceaSystem.getCurrentMemoryLoadMb();
        //    Console.WriteLine("Current memory load with old implementation = " + currentMemoryLoad + " Mb");
        //    myArrayList.Clear();
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        myArrayList.Add(new Matrix(700, 1)); // new implementation
        //    }
        //    double newMemoryLoad = REpiceaSystem.getCurrentMemoryLoadMb();
        //    Console.WriteLine("Current memory load with new implementation = " + newMemoryLoad + " Mb");
        //    Assert.IsTrue(currentMemoryLoad > newMemoryLoad * 2);
        //}

        //       @Test
        //   public void deserializationOfAMatrix() throws FileNotFoundException, UnmarshallingException {

        //       String pathname = ObjectUtility.getPackagePath(getClass()) + "serializedMatrix.xml";
        //       //		Matrix myMatrix = new Matrix(2,4);
        //       //		XmlSerializer serializer = new XmlSerializer(pathname);
        //       //		serializer.writeObject(myMatrix);

        //       XmlDeserializer deserializer = new XmlDeserializer(pathname);
        //       Matrix myDeserializedMatrix = (Matrix)deserializer.readObject();

        //       Assert.assertEquals("Testing deserialized matrix nb rows", 2, myDeserializedMatrix.m_iRows);
        //	Assert.assertEquals("Testing deserialized matrix nb cols", 4, myDeserializedMatrix.m_iCols);
        //}

        //   @Test
        //   public void serializationWithAndWithoutCompression() throws MarshallingException
        //   {
        //       Matrix mat = new Matrix(100,100);
        //   String filename1 = ObjectUtility.getPackagePath(getClass()) + "serializedWithCompression.xml";
        //   XmlSerializer ser1 = new XmlSerializer(filename1);
        //   ser1.writeObject(mat);
        //	File file1 = new File(filename1);
        //   long file1size = file1.length();
        //   String filename2 = ObjectUtility.getPackagePath(getClass()) + "serializedWithoutCompression.xml";
        //   XmlSerializer ser2 = new XmlSerializer(filename2, false);
        //   ser2.writeObject(mat);
        //	File file2 = new File(filename2);
        //   long file2size = file2.length();
        //   double ratio = (double)file2size / file1size;
        //   Assert.assertTrue("Testing compression ratio", ratio > 75);
        //}

        //public void speedTestInversionMatrix(int iMax) throws IOException
        //{
        //    String filename = ObjectUtility.getPackagePath(getClass()) + "inversionTimes.csv";
        //    CSVWriter writer = new CSVWriter(new File(filename), false);
        //List<FormatField> fields = new ArrayList<FormatField>();
        //fields.add(new CSVField("dimension"));
        //fields.add(new CSVField("time"));
        //writer.setFields(fields);

        //long startingTime;
        //Object[] record = new Object[2];
        //int size;
        //for (int i = 1; i < iMax; i++)
        //{
        //    size = i * 10;
        //    record[0] = size;
        //    startingTime = System.currentTimeMillis();
        //    Matrix coordinates = new Matrix(size, 1, 0, 1);
        //    //			Matrix rMatrix = StatisticalUtility.constructRMatrix(coordinates, 2, 0.2, TypeMatrixR.LINEAR);
        //    Matrix rMatrix = StatisticalUtility.constructRMatrix(Arrays.asList(new Double[] { 2d, 0.2 }), TypeMatrixR.LINEAR, coordinates);
        //    rMatrix.getInverseMatrix();
        //    record[1] = (System.currentTimeMillis() - startingTime) * .001;
        //    writer.addRecord(record);
        //}
        //writer.close();
        //	}

        //	public void speedTestMatrixMultiplication()
        //{
        //    int i = 1000;
        //    Matrix oMat = Matrix.getIdentityMatrix(i);
        //    long startingTime;
        //    startingTime = System.currentTimeMillis();
        //    oMat.multiply(oMat);
        //    System.out.println("Elapsed time = " + ((System.currentTimeMillis() - startingTime) * .001));
        //}

        [TestMethod]
        public void Test06ColumnVectorEqualToItself()
        {
            Matrix m1 = new(10, 1, 1, 10);
            Assert.IsTrue(m1.Equals(m1));
        }

        [TestMethod]
        public void Test07TwoColumnVectorsEqual()
        {
            Matrix m1 = new(10, 1, 1, 10);
            Matrix m2 = new(10, 1, 1, 10);
            Assert.IsTrue(m1.Equals(m2));
        }

    }
}
