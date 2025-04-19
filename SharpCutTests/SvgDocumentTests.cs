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

            string svg = document.Export("mm");

            Assert.IsTrue(svg.Contains("<svg"));
            Assert.IsTrue(svg.Contains("M 0 0 L 10 0"));
            Assert.IsTrue(svg.Contains("M 10 0 L 10 10"));
            Assert.IsTrue(svg.Contains("M 10 10 L 0 10"));
            Assert.IsTrue(svg.Contains("M 0 10 L 0 0"));
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

            string svg = document.Export("mm");

            int pathCount = svg.Split("<path").Length - 1;
            Assert.AreEqual(8, pathCount);
        }

        [TestMethod]
        public void Export_UsesCorrectCanvasSize()
        {
            SvgDocument document = new SvgDocument(123, 456);
            string svg = document.Export("mm");

            Assert.IsTrue(svg.Contains("width=\"123.00mm\""));
            Assert.IsTrue(svg.Contains("height=\"456.00mm\""));
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 123 456\""));
        }

        [TestMethod]
        public void Export_RespectsStrokeWidth()
        {
            SvgDocument document = new SvgDocument(100, 100, strokeWidth: 3.5f);
            string svg = document.Export("mm");

            Assert.IsTrue(svg.Contains("stroke-width=\"3.5\""));
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

            string svg = document.Export("mm");

            string expected = """
            <svg xmlns="http://www.w3.org/2000/svg" width="50.00mm" height="70.00mm" viewBox="0 0 50 70">
            <g fill="none" stroke="black" stroke-width="1">
            <path d="M 0 0 L 10 0" />
            <path d="M 30 0 L 40 0" />
            <path d="M 40 0 L 40 60" />
            <path d="M 40 60 L 0 60" />
            <path d="M 0 60 L 0 0" />
            <path d="M 30 0 L 30 30" />
            <path d="M 30 30 L 10 30" />
            <path d="M 10 30 L 10 0" />
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
            Assert.AreEqual(new Point(5, 5), edge.Start);
            Assert.AreEqual(new Point(15, 5), edge.End);
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
            Assert.AreEqual(30.0f + 10.0f - 10.0f, document.Width);  // maxX - minX + margin*2
            Assert.AreEqual(50.0f - 20.0f + 10.0f, document.Height); // maxY - minY + margin*2
        }

        [TestMethod]
        public void ResizeToFitContent_WithOffset_OffsetsSingleShapeAndResizesCanvas()
        {
            Shape shape = new Shape(new List<Edge>
            {
                new Edge(new Point(10, 20), new Point(30, 50))
            });

            SvgDocument document = new SvgDocument(0, 0);
            document.Add(shape, copy: true);
            document.ResizeToFitContent(5, offsetContent: true);

            Assert.AreEqual(30.0f, document.Width);  // 30 - 10 + 2 * 5
            Assert.AreEqual(40.0f, document.Height); // 50 - 20 + 2 * 5

            Edge edge = document.Shapes[0].Edges[0];
            Assert.AreEqual(new Point(5, 5), edge.Start);
            Assert.AreEqual(new Point(25, 35), edge.End);
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

            SvgDocument document = new SvgDocument(0, 0);
            document.Add(shape1, copy: true);
            document.Add(shape2, copy: true);
            document.ResizeToFitContent(5, offsetContent: true);

            // Canvas size = bounding box (0,0) to (40,60) + 2*margin
            Assert.AreEqual(50.0f, document.Width);
            Assert.AreEqual(70.0f, document.Height);

            // Get the offset versions from inside the document
            Edge edge1 = document.Shapes[0].Edges[0];
            Assert.AreEqual(new Point(5, 5), edge1.Start);
            Assert.AreEqual(new Point(15, 5), edge1.End);

            Edge edge2 = document.Shapes[1].Edges[0];
            Assert.AreEqual(new Point(25, 35), edge2.Start);
            Assert.AreEqual(new Point(45, 65), edge2.End);
        }

        [TestMethod]
        public void CreateSizeReference()
        {
            Rectangle rectangle = new Rectangle(0, 0, 120, 30);

            SvgDocument document = new SvgDocument(0, 0, 1);
            document.Add(rectangle);

            string file = document.Export("mm");

            Assert.IsTrue(file.Length > 0);
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

            string exportedSvg = svg.Export();

            File.WriteAllText("advanced_shape.svg", exportedSvg);

            const string expected = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""170.00mm"" height=""60.00mm"" viewBox=""0 0 170 60"">
<g fill=""none"" stroke=""black"" stroke-width=""0.1"">
<path d=""M 5 5 L 165 5"" />
<path d=""M 165 5 L 165 55"" />
<path d=""M 5 55 L 56.83333 55"" />
<path d=""M 59.83333 55 L 110.166664 55"" />
<path d=""M 113.166664 55 L 165 55"" />
<path d=""M 5 55 L 5 5"" />
<path d=""M 110.166664 30 L 113.166664 30"" />
<path d=""M 113.166664 30 L 113.166664 55"" />
<path d=""M 110.166664 55 L 110.166664 30"" />
<path d=""M 56.83333 30 L 59.83333 30"" />
<path d=""M 59.83333 30 L 59.83333 55"" />
<path d=""M 56.83333 55 L 56.83333 30"" />
</g>
</svg>
";

            string normalizedActual = exportedSvg.Replace("\r\n", "\n").Trim();
            string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

            Assert.AreEqual(normalizedExpected, normalizedActual);
        }
    }
}
