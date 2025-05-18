using SharpCut;
using SharpCut.Models;

namespace SharpCutTests
{
    [TestClass]
    public class SvgDocumentTests
    {
        [TestMethod]
        public void AddShape_WithoutCopy_OffsetsOriginalShape()
        {
            Shape shape = new Shape(new List<Edge>
            {
                new Edge(new Point(10, 10), new Point(20, 10))
            });

            SvgDocument document = new SvgDocument(0, 0);
            document.Add(shape);
            document.ResizeToFitContent(5, offsetContent: true);

            // Original shape is now offset too
            Edge edge = shape.Edges[0];
            Assert.AreEqual(new Point(5.5f, 5.5f), edge.Start);
            Assert.AreEqual(new Point(15.5f, 5.5f), edge.End);
        }

        [TestMethod]
        public void AddShape_WithCopy_KeepsOriginalUnchanged()
        {
            Shape shape = new Shape(new List<Edge>
            {
                new Edge(new Point(10, 10), new Point(20, 10))
            });

            SvgDocument document = new SvgDocument(0, 0);
            document.Add(shape, copy: true);
            document.ResizeToFitContent(5, offsetContent: true);

            // Original shape is not modified
            Edge edge = shape.Edges[0];
            Assert.AreEqual(new Point(10, 10), edge.Start);
            Assert.AreEqual(new Point(20, 10), edge.End);
        }

        [TestMethod]
        public void ResizeToFitContent_ComputesCorrectSize()
        {
            Shape shape = new Shape(new List<Edge>
            {
                new Edge(new Point(10, 20), new Point(30, 50))
            });

            SvgDocument document = new SvgDocument(0, 0);
            document.Add(shape, copy: true);
            document.ResizeToFitContent(5, offsetContent: false);

            // Expected bounds: (10,20) to (30,50)
            // Width = 20, Height = 30, Margin = 5 => +10 total
            Assert.AreEqual(31.0f, document.Width);
            Assert.AreEqual(41.0f, document.Height);
        }

        [TestMethod]
        public void ResizeToFitContent_WithOffset_OffsetsSingleShapeAndResizesCanvas()
        {
            Shape shape = new Shape(new List<Edge>
            {
                new Edge(new Point(10, 20), new Point(30, 50))
            });

            SvgDocument document = new SvgDocument(0, 0, strokeWidth: 1);
            document.Add(shape, copy: true);
            document.ResizeToFitContent(5, offsetContent: true);

            // Padding = 5 + 0.5 = 5.5
            // Content size: width = 20, height = 30 → expected width = 20 + 2 * 5.5 = 31
            //                                           expected height = 30 + 2 * 5.5 = 41
            Assert.AreEqual(31.0f, document.Width);
            Assert.AreEqual(41.0f, document.Height);

            // Offset = 5.5 - 10 = -4.5 (X), 5.5 - 20 = -14.5 (Y)
            Edge edge = document.Shapes[0].Edges[0];
            Assert.AreEqual(new Point(5.5f, 5.5f), edge.Start);
            Assert.AreEqual(new Point(25.5f, 35.5f), edge.End);
        }

        [TestMethod]
        public void ResizeToFitContent_WithOffset_OffsetsMultipleShapesAndResizesCanvas()
        {
            Shape shape1 = new Shape(new List<Edge>
            {
                new Edge(new Point(0, 0), new Point(10, 0))
            });

            Shape shape2 = new Shape(new List<Edge>
            {
                new Edge(new Point(20, 30), new Point(40, 60))
            });

            SvgDocument document = new SvgDocument(0, 0, strokeWidth: 1);
            document.Add(shape1, copy: true);
            document.Add(shape2, copy: true);
            document.ResizeToFitContent(5, offsetContent: true);

            // Padding = 5 + 0.5 = 5.5
            // Expected canvas size: width = 40 + 2 * 5.5 = 51
            //                       height = 60 + 2 * 5.5 = 71
            Assert.AreEqual(51.0f, document.Width);
            Assert.AreEqual(71.0f, document.Height);

            // Offset = 5.5 - 0 = 5.5 (applied to all coordinates)
            Edge edge1 = document.Shapes[0].Edges[0];
            Assert.AreEqual(new Point(5.5f, 5.5f), edge1.Start);
            Assert.AreEqual(new Point(15.5f, 5.5f), edge1.End);

            Edge edge2 = document.Shapes[1].Edges[0];
            Assert.AreEqual(new Point(25.5f, 35.5f), edge2.Start);
            Assert.AreEqual(new Point(45.5f, 65.5f), edge2.End);
        }

        [TestMethod]
        public void Add_ListOfShapes_AddsAllShapes()
        {
            Shape shape1 = new Shape(new List<Edge> { new Edge(new Point(0, 0), new Point(10, 0)) });
            Shape shape2 = new Shape(new List<Edge> { new Edge(new Point(20, 0), new Point(30, 0)) });

            SvgDocument document = new SvgDocument(100, 100);
            document.Add(new List<Shape> { shape1, shape2 });

            Assert.AreEqual(2, document.Shapes.Count);
        }       
    }
}
