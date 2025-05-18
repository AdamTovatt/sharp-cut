using SharpCut.Models;

namespace SharpCutTests
{
    [TestClass]
    public class EdgeTests
    {
        [TestMethod]
        public void IsSameEdgeByValue_IndenticalEdges_ReturnsTrue()
        {
            Edge edge1 = new Edge(new Point(10, 15), new Point(90, -20));
            Edge edge2 = new Edge(new Point(10, 15), new Point(90, -20));

            Assert.IsTrue(edge1.IsSameEdgeByValues(edge2));
            Assert.IsTrue(edge2.IsSameEdgeByValues(edge1));
        }

        [TestMethod]
        public void IsSameEdgeByValue_DifferentEdges_ReturnsFalse()
        {
            Edge edge1 = new Edge(new Point(10, 15), new Point(90, -20));
            Edge edge2 = new Edge(new Point(10, 15), new Point(90, 20)); // +20 instead of -20

            Assert.IsFalse(edge1.IsSameEdgeByValues(edge2));
            Assert.IsFalse(edge2.IsSameEdgeByValues(edge1));
        }

        [TestMethod]
        public void IsSameEdgeByValue_CopyOfEdge_ReturnsTrue()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(1, 1));
            Edge copy = edge.Copy();

            Assert.IsTrue(edge.IsSameEdgeByValues(copy));
            Assert.IsTrue(copy.IsSameEdgeByValues(edge));
        }

        [TestMethod]
        public void Subtract_NoOverlap_ReturnsSelf()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            Edge other = new Edge(new Point(20, 0), new Point(30, 0));

            List<Edge> result = edge.Subtract(other);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(edge.Start, result[0].Start);
            Assert.AreEqual(edge.End, result[0].End);
        }

        [TestMethod]
        public void Subtract_ExactMatch_ReturnsEmpty()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            Edge other = new Edge(new Point(0, 0), new Point(10, 0));

            List<Edge> result = edge.Subtract(other);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Subtract_PartialOverlap_ReturnsTrimmedEdge()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            Edge other = new Edge(new Point(5, 0), new Point(15, 0));

            List<Edge> result = edge.Subtract(other);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new Point(0, 0), result[0].Start);
            Assert.AreEqual(new Point(5, 0), result[0].End);
        }

        [TestMethod]
        public void Subtract_ContainedOverlap_ReturnsTwoSegments()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            Edge other = new Edge(new Point(3, 0), new Point(7, 0));

            List<Edge> result = edge.Subtract(other);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new Point(0, 0), result[0].Start);
            Assert.AreEqual(new Point(3, 0), result[0].End);
            Assert.AreEqual(new Point(7, 0), result[1].Start);
            Assert.AreEqual(new Point(10, 0), result[1].End);
        }

        [TestMethod]
        public void Subtract_IsDirectional_SymmetricOverlapTrimsCorrectly()
        {
            Edge a = new Edge(new Point(0, 0), new Point(10, 0));
            Edge b = new Edge(new Point(5, 0), new Point(15, 0));

            List<Edge> resultA = a.Subtract(b);
            List<Edge> resultB = b.Subtract(a);

            // a should be trimmed to left segment
            Assert.AreEqual(1, resultA.Count);
            Assert.AreEqual(new Point(0, 0), resultA[0].Start);
            Assert.AreEqual(new Point(5, 0), resultA[0].End);

            // b should be trimmed to right segment
            Assert.AreEqual(1, resultB.Count);
            Assert.AreEqual(new Point(10, 0), resultB[0].Start);
            Assert.AreEqual(new Point(15, 0), resultB[0].End);
        }

        [TestMethod]
        public void GetDistributedPoints_NoMargins_DistributesEvenly()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(3, includeEndpoints: true);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Point(0, 0), points[0]);
            Assert.AreEqual(new Point(5, 0), points[1]);
            Assert.AreEqual(new Point(10, 0), points[2]);
        }

        [TestMethod]
        public void GetDistributedPoints_WithMargins_SkipsEnds()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(3, startMargin: 2, endMargin: 2, includeEndpoints: true);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Point(2, 0), points[0]);
            Assert.AreEqual(new Point(5, 0), points[1]);
            Assert.AreEqual(new Point(8, 0), points[2]);
        }

        [TestMethod]
        public void GetDistributedPoints_OnePoint_ReturnsMiddleOfUsableLength()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(1, startMargin: 2, endMargin: 2);

            Assert.AreEqual(1, points.Count);
            Assert.AreEqual(new Point(5, 0), points[0]);
        }

        [TestMethod]
        public void GetDistributedPoints_ZeroCount_ReturnsEmpty()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(0);

            Assert.AreEqual(0, points.Count);
        }

        [TestMethod]
        public void GetDistributedPoints_UsableLengthTooSmall_ReturnsMiddlePoint()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(3, startMargin: 5, endMargin: 6); // usable length < 0

            Assert.AreEqual(1, points.Count);
            Assert.AreEqual(new Point(4.5f, 0), points[0]);
        }

        [TestMethod]
        public void GetDistributedPoints_WithoutEndpoints_DistributesInsideMargins()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 0));
            List<Point> points = edge.GetDistributedPoints(3);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Point(2.5f, 0), points[0]);
            Assert.AreEqual(new Point(5.0f, 0), points[1]);
            Assert.AreEqual(new Point(7.5f, 0), points[2]);
        }

        [TestMethod]
        public void GetDistributedPoints_VerticalEdge_DistributesCorrectly()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(0, 10));
            List<Point> points = edge.GetDistributedPoints(3, includeEndpoints: true);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Point(0, 0), points[0]);
            Assert.AreEqual(new Point(0, 5), points[1]);
            Assert.AreEqual(new Point(0, 10), points[2]);
        }

        [TestMethod]
        public void GetDistributedPoints_DiagonalEdge_DistributesCorrectly()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 10));
            List<Point> points = edge.GetDistributedPoints(3, includeEndpoints: true);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Point(0, 0), points[0]);
            Assert.AreEqual(new Point(5, 5), points[1]);
            Assert.AreEqual(new Point(10, 10), points[2]);
        }

        [TestMethod]
        public void GetDistributedPoints_DiagonalEdge_WithMargins_DistributesWithinUsableRange()
        {
            Edge edge = new Edge(new Point(0, 0), new Point(10, 10));
            List<Point> points = edge.GetDistributedPoints(3, startMargin: 2, endMargin: 2, includeEndpoints: true);

            Assert.AreEqual(3, points.Count);

            float expected = (float)(2 / Math.Sqrt(2)); // ~= 1.4142135
            float delta = 0.0001f;

            Assert.AreEqual(expected, points[0].X, delta);
            Assert.AreEqual(expected, points[0].Y, delta);

            Assert.AreEqual(5f, points[1].X, delta);
            Assert.AreEqual(5f, points[1].Y, delta);

            Assert.AreEqual(10 - expected, points[2].X, delta);
            Assert.AreEqual(10 - expected, points[2].Y, delta);
        }
    }
}