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

        [DataTestMethod]
        [DataRow(Origin.TopLeft, 10, 20)]
        [DataRow(Origin.TopCenter, -5, 20)]
        [DataRow(Origin.TopRight, -20, 20)]
        [DataRow(Origin.CenterLeft, 10, 5)]
        [DataRow(Origin.Center, -5, 5)]
        [DataRow(Origin.CenterRight, -20, 5)]
        [DataRow(Origin.BottomLeft, 10, -10)]
        [DataRow(Origin.BottomCenter, -5, -10)]
        [DataRow(Origin.BottomRight, -20, -10)]
        public void SetPosition_AllAnchors_SetsExpectedCoordinates(Origin origin, float expectedX, float expectedY)
        {
            Rectangle rect = new Rectangle(0, 0, 30, 30);
            rect.SetPosition(10, 20, origin);

            Assert.AreEqual(expectedX, rect.X);
            Assert.AreEqual(expectedY, rect.Y);
        }

        [DataTestMethod]
        [DataRow(Origin.TopLeft, 10, 20)]
        [DataRow(Origin.TopCenter, -5, 20)]
        [DataRow(Origin.TopRight, -20, 20)]
        [DataRow(Origin.CenterLeft, 10, 5)]
        [DataRow(Origin.Center, -5, 5)]
        [DataRow(Origin.CenterRight, -20, 5)]
        [DataRow(Origin.BottomLeft, 10, -10)]
        [DataRow(Origin.BottomCenter, -5, -10)]
        [DataRow(Origin.BottomRight, -20, -10)]
        public void CreateFromOrigin_AllAnchors_ProducesExpectedRectangle(Origin origin, float expectedX, float expectedY)
        {
            Rectangle rect = Rectangle.CreateFromOrigin(10, 20, 30, 30, origin);

            Assert.AreEqual(expectedX, rect.X);
            Assert.AreEqual(expectedY, rect.Y);
            Assert.AreEqual(30, rect.Width);
            Assert.AreEqual(30, rect.Height);
        }

        [DataTestMethod]
        [DataRow(Side.Top, 10, 20, 40, 20)]
        [DataRow(Side.Right, 40, 20, 40, 60)]
        [DataRow(Side.Bottom, 40, 60, 10, 60)]
        [DataRow(Side.Left, 10, 60, 10, 20)]
        public void GetEdge_ReturnsCorrectEdgeForSide(Side side, float startX, float startY, float endX, float endY)
        {
            Rectangle rect = new Rectangle(10, 20, 30, 40);
            Edge edge = rect.GetEdge(side);

            Assert.AreEqual(new Point(startX, startY), edge.Start);
            Assert.AreEqual(new Point(endX, endY), edge.End);
        }
    }
}
