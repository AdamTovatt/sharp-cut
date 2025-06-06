﻿using SharpCut.Helpers;
using SharpCut.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SharpCutTests
{
    [TestClass]
    public class PathReaderTests
    {
        [DataTestMethod]
        [DataRow("0", 0)]
        [DataRow("1", 1)]
        [DataRow("2", 2)]
        [DataRow("3", 3)]
        [DataRow("4", 4)]
        [DataRow("5", 5)]
        [DataRow("6", 6)]
        [DataRow("7", 7)]
        [DataRow("8", 8)]
        [DataRow("9", 9)]
        [DataRow("10", 10)]
        [DataRow("11", 11)]
        [DataRow("24", 24)]
        [DataRow("894", 894)]
        [DataRow("991205", 991205)]

        public void ReadFloat_WithoutDecimalPoint_ReadsCorrectValue(string path, float expectedValue)
        {
            using (PathReader pathReader = new PathReader(path))
            {
                float? readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(expectedValue, readResult);

                Assert.IsNull(pathReader.ReadFloat());
            }
        }

        [DataTestMethod]
        [DataRow("0.2", 0.2f)]
        [DataRow(".44", 0.44f)]
        [DataRow("1.635", 1.635f)]
        [DataRow("500.10", 500.1f)]
        [DataRow("100.02", 100.02f)]
        [DataRow("0.0002", 0.0002f)]
        [DataRow("420000.19876", 420000.19876f)]
        public void ReadFloat_WithDecimalPoint_ReadsCorrectValue(string path, float expectedValue)
        {
            using (PathReader pathReader = new PathReader(path))
            {
                float? readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(expectedValue, readResult);

                Assert.IsNull(pathReader.ReadFloat());
            }
        }

        [DataTestMethod]
        [DataRow("5 1", 5f, 1f)]
        [DataRow("10.2 10", 10.2f, 10f)]
        [DataRow("1.635 1635", 1.635f, 1635f)]
        [DataRow(".11 25", 0.11f, 25f)]
        [DataRow("100.02 100", 100.02f, 100)]
        [DataRow("0.0002 0.05", 0.0002f, 0.05f)]
        [DataRow("420000.19876 24355.2112", 420000.19876f, 24355.2112f)]
        public void ReadFloat_TwoFloatString_ReadsCorrectValue(string path, float firstExpectedValue, float secondExpectedValue)
        {
            using (PathReader pathReader = new PathReader(path))
            {
                float? readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(firstExpectedValue, readResult);

                pathReader.Read();
                readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(secondExpectedValue, readResult);

                Assert.IsNull(pathReader.ReadFloat());
            }
        }

        [DataTestMethod]
        [DataRow("0 0 0", 0f, 0f, 0f)]
        [DataRow("1 1 1", 1f, 1f, 1f)]
        [DataRow(".11 25 42.1", 0.11f, 25f, 42.1f)]
        [DataRow("100.02 100 1", 100.02f, 100f, 1f)]
        public void ReadFloat_ThreeFloatString_ReadsCorrectValue(
            string path,
            float firstExpectedValue,
            float secondExpectedValue,
            float thirdExpectedValue)
        {
            using (PathReader pathReader = new PathReader(path))
            {
                float? readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(firstExpectedValue, readResult);

                pathReader.Read();
                readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(secondExpectedValue, readResult);

                pathReader.Read();
                readResult = pathReader.ReadFloat();

                Assert.IsNotNull(readResult);
                Assert.AreEqual(thirdExpectedValue, readResult);

                Assert.IsNull(pathReader.ReadFloat());
            }
        }

        [TestMethod]
        public void ReadFloat_RandomGeneratedString_ReadsCorrectValue()
        {
            const int valueCount = 1000;

            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random(1);

            float[] expectedValues = new float[valueCount];

            for (int i = 0; i < valueCount; i++)
            {
                float value = random.NextSingle() * (random.NextSingle() * 100);
                expectedValues[i] = value;
                stringBuilder.Append($"{value.ToString(CultureInfo.InvariantCulture)} ");
            }

            string path = stringBuilder.ToString();
            Console.WriteLine(path);

            float?[] actualValues = new float?[valueCount];

            using (PathReader reader = new PathReader(path))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < valueCount; i++)
                {
                    actualValues[i] = reader.ReadFloat();
                    reader.Read();
                }

                stopwatch.Stop();

                Console.WriteLine($"Average read time: {stopwatch.Elapsed.TotalNanoseconds / (float)valueCount} ns");
            }

            for (int i = 0; i < valueCount; i++)
            {
                Assert.IsNotNull(actualValues[i]);
                Assert.AreEqual(expectedValues[i], actualValues[i]!.Value, delta: 0.0001f);
            }
        }

        [TestMethod]
        public void ReadPath_AffinityExportedPath_ReadsCorrectPoints()
        {
            const string pathData = "M929.563,816.175L184.116,816.175L184.116,160.541L313.222,160.541L313.222,499.584L355.884,499.584L355.884,160.541L540,160.541L540,499.584L582.661,499.584L582.661,160.541L739.834,160.541L739.834,410.894L929.563,410.894L929.563,816.175Z";

            using (PathReader reader = new PathReader(pathData))
            {
                reader.ReadStartOfPath();

                List<Point> points = reader.ReadPointListFromPath(out bool didReadCloseCharacter);

                Assert.IsTrue(didReadCloseCharacter);

                Assert.AreEqual(929.563, points[0].X, 0.1);
                Assert.AreEqual(816.175, points[0].Y, 0.1);

                Assert.AreEqual(184.116, points[1].X, 0.1);
                Assert.AreEqual(816.175, points[1].Y, 0.1);

                Assert.AreEqual(184.116, points[2].X, 0.1);
                Assert.AreEqual(160.541, points[2].Y, 0.1);
            }
        }

        [TestMethod]
        public void ReadPath_SelfExportedPath_ReadsCorrectPoints()
        {
            const string pathData = "M 929.563 816.175 L 184.116 816.175 L 184.116 160.541 L 313.22202 160.541 L 313.22202 499.58398 L 355.88397 499.58398 L 355.88397 160.541 L 540 160.541 L 540 499.58398 L 582.66095 499.58398 L 582.66095 160.541 L 739.83405 160.541 L 739.83405 410.89398 L 929.563 410.89398 Z";

            using (PathReader reader = new PathReader(pathData))
            {
                reader.ReadStartOfPath();

                List<Point> points = reader.ReadPointListFromPath(out bool didReadCloseCharacter);

                Assert.IsTrue(didReadCloseCharacter);

                Assert.AreEqual(929.563, points[0].X, 0.1);
                Assert.AreEqual(816.175, points[0].Y, 0.1);

                Assert.AreEqual(184.116, points[1].X, 0.1);
                Assert.AreEqual(816.175, points[1].Y, 0.1);

                Assert.AreEqual(184.116, points[2].X, 0.1);
                Assert.AreEqual(160.541, points[2].Y, 0.1);
            }
        }
    }
}
