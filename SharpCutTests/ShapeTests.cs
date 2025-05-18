using SharpCut.Models;

namespace SharpCutTests
{
    [TestClass]
    public class ShapeTests
    {
        [TestMethod]
        public void GetClosedPaths_Square_ReturnsSingleClosedPath()
        {
            // Arrange
            List<Edge> edges = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)),
                new Edge(new Point(10, 10), new Point(0, 10)),
                new Edge(new Point(0, 10), new Point(0, 0))
            };

            Shape shape = new Shape(edges);

            // Act
            List<List<Point>> paths = shape.GetClosedPaths();

            // Assert
            Assert.AreEqual(1, paths.Count);

            List<Point> path = paths[0];
            Assert.AreEqual(4, path.Count);
            Assert.AreEqual(new Point(0, 0), path[0]);
            Assert.AreEqual(new Point(10, 0), path[1]);
            Assert.AreEqual(new Point(10, 10), path[2]);
            Assert.AreEqual(new Point(0, 10), path[3]);
        }

        [TestMethod]
        public void IsSameShapeByValues_SameSquare_ReturnsTrue()
        {
            List<Edge> edges = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)),
                new Edge(new Point(10, 10), new Point(0, 10)),
                new Edge(new Point(0, 10), new Point(0, 0))
            };

            Shape original = new Shape(edges);

            Shape copy1 = new Shape(original);
            Shape copy2 = original.Copy();

            Assert.IsTrue(original.IsSameShapeByValues(copy1));
            Assert.IsTrue(original.IsSameShapeByValues(copy2));

            Assert.IsTrue(copy1.IsSameShapeByValues(original));
            Assert.IsTrue(copy1.IsSameShapeByValues(copy2));

            Assert.IsTrue(copy2.IsSameShapeByValues(original));
            Assert.IsTrue(copy2.IsSameShapeByValues(copy1));
        }

        [TestMethod]
        public void IsSameShapeByValues_DifferentSquare_ReturnsFalse()
        {
            List<Edge> edges1 = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)),
                new Edge(new Point(10, 10), new Point(0, 10)),
                new Edge(new Point(0, 10), new Point(0, 0))
            };

            List<Edge> edges2 = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(12, 0)),
                new Edge(new Point(12, 0), new Point(12, 12)),
                new Edge(new Point(10, 12), new Point(0, 12)),
                new Edge(new Point(0, 12), new Point(0, 0))
            };

            Shape square1 = new Shape(edges1);
            Shape square2 = new Shape(edges2);

            Assert.IsFalse(square1.IsSameShapeByValues(square2));
            Assert.IsFalse(square2.IsSameShapeByValues(square1));
        }

        [TestMethod]
        public void IsSameShapeByValues_SameSquareButDuplicatedEdge_ReturnsFalse()
        {
            List<Edge> edges1 = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)),
                new Edge(new Point(10, 10), new Point(0, 10)),
                new Edge(new Point(0, 10), new Point(0, 0)),
            };

            List<Edge> edges2 = new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)),
                new Edge(new Point(10, 10), new Point(0, 10)),
                new Edge(new Point(0, 10), new Point(0, 0)),
                new Edge(new Point(10, 0), new Point(10, 10)), // duplicated edge
            };

            Shape square1 = new Shape(edges1);
            Shape square2 = new Shape(edges2);

            Assert.IsFalse(square1.IsSameShapeByValues(square2));
            Assert.IsFalse(square2.IsSameShapeByValues(square1));
        }
    }
}
