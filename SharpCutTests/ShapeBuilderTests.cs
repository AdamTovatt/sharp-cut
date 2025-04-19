using SharpCut.Builders;
using SharpCut.Models;

namespace SharpCut.Tests
{
    [TestClass]
    public class ShapeBuilderTests
    {
        [TestMethod]
        public void Build_Ordered_RemovesLaterOverlappingEdge()
        {
            Rectangle rect1 = new Rectangle(0, 0, 10, 10);
            Rectangle rect2 = new Rectangle(10, 0, 10, 10); // Shares vertical edge with rect1

            List<IShape> shapes = new List<IShape> { rect1, rect2 };

            Shape result = ShapeBuilder.Build(shapes, symmetrical: false);

            // One shared vertical edge should be removed from rect2
            Assert.AreEqual(7, result.Edges.Count); // 4 from rect1, 3 from rect2
        }

        [TestMethod]
        public void Build_Symmetrical_RemovesSharedEdgeFromBoth()
        {
            Rectangle rect1 = new Rectangle(0, 0, 10, 10);
            Rectangle rect2 = new Rectangle(10, 0, 10, 10); // Shares vertical edge with rect1

            List<IShape> shapes = new List<IShape> { rect1, rect2 };

            Shape result = ShapeBuilder.Build(shapes, symmetrical: true);

            // Shared edge is removed from both rectangles
            Assert.AreEqual(6, result.Edges.Count); // 3 + 3
        }

        [TestMethod]
        public void Build_EmptyInput_ReturnsEmptyShape()
        {
            List<IShape> shapes = new List<IShape>();

            Shape result = ShapeBuilder.Build(shapes);

            Assert.AreEqual(0, result.Edges.Count);
        }

        [TestMethod]
        public void Build_NonOverlappingEdges_AllPreserved()
        {
            Rectangle rect1 = new Rectangle(0, 0, 10, 10);
            Rectangle rect2 = new Rectangle(20, 0, 10, 10);

            List<IShape> shapes = new List<IShape> { rect1, rect2 };

            Shape result = ShapeBuilder.Build(shapes, symmetrical: true);

            Assert.AreEqual(8, result.Edges.Count); // 4 + 4, no overlap
        }
    }
}
