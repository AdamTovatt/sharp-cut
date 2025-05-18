using SharpCut.Models;

namespace SharpCutTests
{
    [TestClass]
    public class PointTests
    {
        [TestMethod]
        public void Equality_Operator_ReturnsTrueForEqualPoints()
        {
            Point a = new Point(5, 10);
            Point b = new Point(5, 10);

            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void Equality_Operator_ReturnsFalseForDifferentPoints()
        {
            Point a = new Point(5, 10);
            Point b = new Point(10, 5);

            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void Addition_Operator_AddsCoordinates()
        {
            Point a = new Point(2, 3);
            Point b = new Point(4, 5);

            Point result = a + b;

            Assert.AreEqual(new Point(6, 8), result);
        }

        [TestMethod]
        public void Subtraction_Operator_SubtractsCoordinates()
        {
            Point a = new Point(10, 7);
            Point b = new Point(3, 2);

            Point result = a - b;

            Assert.AreEqual(new Point(7, 5), result);
        }

        [TestMethod]
        public void Equals_ReturnsTrueForSameCoordinates()
        {
            Point a = new Point(1, 2);
            Point b = new Point(1, 2);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
        }

        [TestMethod]
        public void Equals_ReturnsFalseForDifferentCoordinates()
        {
            Point a = new Point(1, 2);
            Point b = new Point(2, 1);

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
        }

        [TestMethod]
        public void GetHashCode_SamePoints_HaveSameHash()
        {
            Point a = new Point(10, 20);
            Point b = new Point(10, 20);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void HashSet_UsesPointEquality()
        {
            HashSet<Point> set = new HashSet<Point>();
            set.Add(new Point(1, 1));

            Assert.IsTrue(set.Contains(new Point(1, 1)));
            Assert.IsFalse(set.Contains(new Point(2, 2)));
        }
    }
}
