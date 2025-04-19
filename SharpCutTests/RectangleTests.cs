using SharpCut.Models;

namespace SharpCut.Tests
{
    [TestClass]
    public class RectangleTests
    {
        [TestMethod]
        public void GetEdges_ReturnsFourCorrectEdges()
        {
            Rectangle rect = new Rectangle(10, 20, 30, 40);
            List<Edge> edges = rect.GetEdges();

            Assert.AreEqual(4, edges.Count);
            Assert.AreEqual(new Point(10, 20), edges[0].Start); // Top
            Assert.AreEqual(new Point(40, 20), edges[0].End);

            Assert.AreEqual(new Point(40, 20), edges[1].Start); // Right
            Assert.AreEqual(new Point(40, 60), edges[1].End);

            Assert.AreEqual(new Point(40, 60), edges[2].Start); // Bottom
            Assert.AreEqual(new Point(10, 60), edges[2].End);

            Assert.AreEqual(new Point(10, 60), edges[3].Start); // Left
            Assert.AreEqual(new Point(10, 20), edges[3].End);
        }

        [TestMethod]
        public void CreateFromOrigin_TopLeft_ProducesExpectedRectangle()
        {
            Rectangle rect = Rectangle.CreateFromOrigin(10, 20, 30, 40, Origin.TopLeft);

            Assert.AreEqual(10, rect.X);
            Assert.AreEqual(20, rect.Y);
            Assert.AreEqual(30, rect.Width);
            Assert.AreEqual(40, rect.Height);
        }

        [TestMethod]
        public void CreateFromOrigin_BottomRight_ProducesExpectedRectangle()
        {
            Rectangle rect = Rectangle.CreateFromOrigin(40, 60, 30, 40, Origin.BottomRight);

            Assert.AreEqual(10, rect.X);
            Assert.AreEqual(20, rect.Y);
            Assert.AreEqual(30, rect.Width);
            Assert.AreEqual(40, rect.Height);
        }

        [TestMethod]
        public void ToString_ReturnsReadableString()
        {
            Rectangle rect = new Rectangle(5, 10, 20, 25);
            string result = rect.ToString();

            Assert.AreEqual("Rectangle: (5, 10) 20x25", result);
        }
    }
}
