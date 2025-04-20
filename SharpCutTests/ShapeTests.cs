using SharpCut.Models;

namespace SharpCut.Tests
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
    }
}
