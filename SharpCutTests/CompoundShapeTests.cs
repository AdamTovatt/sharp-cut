using SharpCut.Models;

namespace SharpCut.Tests
{
    [TestClass]
    public class CompoundShapeTests
    {
        [TestMethod]
        public void CompoundShape_SingleShape_ReturnsEdges()
        {
            Rectangle rect = new Rectangle(0, 0, 10, 10);
            CompoundShape compound = new CompoundShape(rect);

            List<Edge> edges = compound.GetEdges();

            Assert.AreEqual(4, edges.Count);
        }

        [TestMethod]
        public void CompoundShape_MultipleShapes_RemovesSharedEdges()
        {
            Rectangle left = new Rectangle(0, 0, 10, 10);
            Rectangle right = new Rectangle(10, 0, 10, 10); // Shares edge with left

            CompoundShape compound = new CompoundShape(left, right);
            List<Edge> edges = compound.GetEdges();

            // Each rectangle has 4 edges, 1 shared edge removed symmetrically → 6 remain
            Assert.AreEqual(6, edges.Count);
        }

        [TestMethod]
        public void CompoundShape_AddSingle_AddsCorrectly()
        {
            CompoundShape compound = new CompoundShape();
            Rectangle rect = new Rectangle(0, 0, 10, 10);

            compound.Add(rect);
            Assert.AreEqual(4, compound.GetEdges().Count);
        }

        [TestMethod]
        public void CompoundShape_AddMultiple_AddsCorrectly()
        {
            CompoundShape compound = new CompoundShape();
            Rectangle r1 = new Rectangle(0, 0, 10, 10);
            Rectangle r2 = new Rectangle(20, 0, 10, 10);

            compound.Add(new List<IShape> { r1, r2 });

            List<Edge> edges = compound.GetEdges();
            Assert.AreEqual(8, edges.Count); // no overlap
        }

        [TestMethod]
        public void CompoundShape_ToString_ReportsCorrectCount()
        {
            CompoundShape compound = new CompoundShape();
            compound.Add(new Rectangle(0, 0, 10, 10));
            compound.Add(new Rectangle(20, 0, 10, 10));

            string str = compound.ToString();
            Assert.IsTrue(str.Contains("2 shape"));
        }
    }
}
