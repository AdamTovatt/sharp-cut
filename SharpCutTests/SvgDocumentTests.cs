using SharpCut.Builders;
using SharpCut.Models;

namespace SharpCut.Tests
{
    [TestClass]
    public class SvgDocumentTests
    {
        [TestMethod]
        public void Export_SingleShape_ContainsExpectedPathCommands()
        {
            Rectangle rect = new Rectangle(0, 0, 10, 10);
            Shape shape = new Shape(rect.GetEdges());

            SvgDocument document = new SvgDocument(100, 100);
            document.Add(shape);

            string svg = document.Export();

            Assert.IsTrue(svg.Contains("<svg"));
            Assert.IsTrue(svg.Contains("M 0 0 L 10 0 L 10 10 L 0 10 Z"));
        }

        [TestMethod]
        public void Export_MultipleShapes_ContainsAllEdges()
        {
            Rectangle rect1 = new Rectangle(0, 0, 10, 10);
            Rectangle rect2 = new Rectangle(20, 0, 10, 10);

            Shape shape1 = new Shape(rect1.GetEdges());
            Shape shape2 = new Shape(rect2.GetEdges());

            SvgDocument document = new SvgDocument(100, 100);
            document.Add(shape1);
            document.Add(shape2);

            string svg = document.Export();

            int pathCount = svg.Split("<path").Length - 1;
            Assert.AreEqual(2, pathCount); // should only be two paths that make up the squares
        }

        [TestMethod]
        public void Export_UsesCorrectCanvasSize()
        {
            SvgDocument document = new SvgDocument(123, 456);
            string svg = document.Export();

            Assert.IsTrue(svg.Contains("width=\"123.00mm\""));
            Assert.IsTrue(svg.Contains("height=\"456.00mm\""));
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 123 456\""));
        }

        [TestMethod]
        public void Export_RespectsStrokeWidth()
        {
            SvgDocument document = new SvgDocument(100, 100, strokeWidth: 3.5f);
            string svg = document.Export();

            Assert.IsTrue(svg.Contains("stroke-width=\"3.5\""));
        }

        [TestMethod]
        public void Export_SimpleRectangle_ProducesExpectedSvgMarkup()
        {
            Rectangle rectangle = new Rectangle(10, 10, 120, 120);

            SvgDocument document = new SvgDocument();

            document.Add(rectangle, true);

            document.ResizeToFitContent(5, true);
            string svg = document.Export();

            File.WriteAllText("simple-rectangle.svg", svg);

            const string expected = """
                <svg xmlns="http://www.w3.org/2000/svg" width="131.00mm" height="131.00mm" viewBox="0 0 131 131">
                <g fill="none" stroke="black" stroke-width="1">
                <path d="M 5.5 5.5 L 125.5 5.5 L 125.5 125.5 L 5.5 125.5 Z" />
                </g>
                </svg>
                """;

            string normalizedActual = svg.Replace("\r\n", "\n").Trim();
            string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

            Assert.AreEqual(normalizedExpected, normalizedActual);
        }

        [TestMethod]
        public void Export_BlockUFlipped_ProducesExpectedSvgMarkup()
        {
            Rectangle outer = new Rectangle(0, 0, 40, 60);
            Rectangle cutout = new Rectangle(10, 0, 20, 30);

            List<IShape> shapes = new List<IShape> { outer, cutout };

            Shape finalShape = ShapeBuilder.Build(shapes, symmetrical: true);

            SvgDocument document = new SvgDocument(50, 70);
            document.Add(finalShape);

            string svg = document.Export();

            File.WriteAllText("block-u-flipped.svg", svg);

            string expected = """
                <svg xmlns="http://www.w3.org/2000/svg" width="50.00mm" height="70.00mm" viewBox="0 0 50 70">
                <g fill="none" stroke="black" stroke-width="1">
                <path d="M 0 0 L 10 0 L 10 30 L 30 30 L 30 0 L 40 0 L 40 60 L 0 60 Z" />
                </g>
                </svg>
                """;

            string normalizedActual = svg.Replace("\r\n", "\n").Trim();
            string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

            Assert.AreEqual(normalizedExpected, normalizedActual);
        }

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
        public void Export_WithResizeToFitContent_DoesNotClipRightOrBottomEdges()
        {
            // Arrange
            Rectangle rect = new Rectangle(10, 10, 120, 120);
            SvgDocument document = new SvgDocument();
            document.Add(rect, copy: true);
            document.ResizeToFitContent(margin: 5, offsetContent: false); // Explicit call to resize

            // Act
            string svg = document.Export();

            // Assert - width/height should be 120 + 2 * (5 + 0.5) = 131.0
            Assert.IsTrue(svg.Contains("width=\"131.00mm\""));
            Assert.IsTrue(svg.Contains("height=\"131.00mm\""));
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 131 131\""));

            // Confirm edge presence
            Assert.IsTrue(svg.Contains("L 130 10"), "Expected right edge not present");
            Assert.IsTrue(svg.Contains("L 130 130"), "Expected bottom-right corner not present");
            Assert.IsTrue(svg.Contains("L 10 130"), "Expected bottom edge not present");
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

        [TestMethod]
        public void Export_AdvancedShape_ProducesExpectedSvgMarkup()
        {
            Rectangle panelBase = new Rectangle(x: 5, y: 5, width: 160, height: 50);

            Rectangle cut = new Rectangle(width: 3, height: 25);

            List<Rectangle> placedCuts = cut.PlaceCopiesOnPoints(
                points: panelBase.GetEdge(Side.Bottom).GetDistributedPoints(2),
                origin: Origin.BottomCenter);

            SvgDocument svg = new SvgDocument(strokeWidth: 0.1f);

            CompoundShape compoundShape = new CompoundShape(panelBase, placedCuts);
            svg.Add(compoundShape);

            svg.ResizeToFitContent(5, true);
            string exportedSvg = svg.Export();

            File.WriteAllText("advanced_shape.svg", exportedSvg);

            const string expected = """
                <svg xmlns="http://www.w3.org/2000/svg" width="170.10mm" height="60.10mm" viewBox="0 0 170.1 60.1">
                <g fill="none" stroke="black" stroke-width="0.1">
                <path d="M 5.05 5.05 L 165.05 5.05 L 165.05 55.05 L 113.21667 55.05 L 113.21667 30.05 L 110.21667 30.05 L 110.21667 55.05 L 59.883327 55.05 L 59.883327 30.05 L 56.883327 30.05 L 56.883327 55.05 L 5.05 55.05 Z" />
                </g>
                </svg>
                """;

            string normalizedActual = exportedSvg.Replace("\r\n", "\n").Trim();
            string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

            Assert.AreEqual(normalizedExpected, normalizedActual);
        }
    }
}
