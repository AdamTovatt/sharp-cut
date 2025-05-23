using SharpCut.Builders;
using SharpCut.Models;
using SharpCut;

namespace SharpCutTests
{
    [TestClass]
    public class SvgExportImportTests
    {
        private SvgDocument CreateSingleShapeDocument()
        {
            Rectangle rect = new Rectangle(0, 0, 10, 10);
            Shape shape = new Shape(rect.GetEdges());

            SvgDocument document = new SvgDocument(100, 100);
            document.Add(shape);

            return document;
        }

        [TestMethod]
        public void Export_SingleShape_ContainsExpectedPathCommands()
        {
            SvgDocument document = CreateSingleShapeDocument();
            string svg = document.Export();

            Assert.IsTrue(svg.Contains("<svg"));
            Assert.IsTrue(svg.Contains("M 0 0 L 10 0 L 10 10 L 0 10 Z"));
        }

        [TestMethod]
        public void Import_SingleShape_ContainsExpectedShapes()
        {
            SvgDocument document = CreateSingleShapeDocument();
            string svg = document.Export();

            Console.WriteLine(svg);

            SvgDocument importedDocument = SvgDocument.Import(svg);

            Assert.AreEqual(1, importedDocument.Shapes.Count);

            Shape shape = importedDocument.Shapes[0];

            Assert.AreEqual(4, shape.Edges.Count);

            Assert.AreEqual(shape, shape);

            string secondExport = importedDocument.Export();

            Console.WriteLine();
            Console.WriteLine(secondExport);

            Assert.AreEqual(svg, secondExport);
        }

        [TestMethod]
        public void Import_FileFromAffinity_ContainsExpectedShapes()
        {
            const string affinitySvg = """
                <?xml version="1.0" encoding="UTF-8" standalone="no"?>
                <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">
                <svg width="100%" height="100%" viewBox="0 0 1080 1080" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" xml:space="preserve" xmlns:serif="http://www.serif.com/" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:1.5;">
                    <g transform="matrix(1.16893,0,0,1.16893,-110.907,-30.8557)">
                        <path d="M929.563,816.175L184.116,816.175L184.116,160.541L313.222,160.541L313.222,499.584L355.884,499.584L355.884,160.541L540,160.541L540,499.584L582.661,499.584L582.661,160.541L739.834,160.541L739.834,410.894L929.563,410.894L929.563,816.175Z" style="fill:rgb(235,235,235);stroke:black;stroke-width:8.55px;"/>
                    </g>
                </svg>                
                """;

            SvgDocument imported = SvgDocument.Import(affinitySvg);

            Assert.AreEqual(1, imported.Shapes.Count);

            string exportedSvg = imported.Export();

            File.WriteAllText("affinity-svg.svg", exportedSvg);
            Console.WriteLine(exportedSvg);
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

            Console.WriteLine(svg);

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
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 123.00 456.00\""));
        }

        [TestMethod]
        public void Import_UsesCorrectCanvasSize()
        {
            SvgDocument document = new SvgDocument(123, 456);
            string svg = document.Export();

            SvgDocument importedDocument = SvgDocument.Import(svg);

            Assert.IsNotNull(importedDocument);
            Assert.AreEqual(123, importedDocument.Attributes.Width);
            Assert.AreEqual(456, importedDocument.Attributes.Height);
            Assert.AreEqual("mm", importedDocument.Attributes.Unit);
        }

        [TestMethod]
        public void Export_RespectsStrokeWidth()
        {
            SvgDocument document = new SvgDocument(100, 100, strokeWidth: 3.5f);
            string svg = document.Export();

            Assert.IsTrue(svg.Contains("stroke-width=\"3.5\""));
        }

        [TestMethod]
        public void Import_RespectsStrokeWidth()
        {
            SvgDocument document = new SvgDocument(100, 100, strokeWidth: 3.5f);
            string svg = document.Export();

            SvgDocument importedDocument = SvgDocument.Import(svg);

            Assert.IsNotNull(importedDocument);
            Assert.AreEqual(3.5f, importedDocument.Attributes.StrokeWidth);
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
                <svg xmlns="http://www.w3.org/2000/svg" width="131.00mm" height="131.00mm" viewBox="0 0 131.00 131.00">
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
                <svg xmlns="http://www.w3.org/2000/svg" width="50.00mm" height="70.00mm" viewBox="0 0 50.00 70.00">
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
        public void Export_WithResizeToFitContent_DoesNotClipRightOrBottomEdges()
        {
            // Arrange
            Rectangle rect = new Rectangle(10, 10, 120, 120);
            SvgDocument document = new SvgDocument();
            document.Add(rect, copy: true);
            document.ResizeToFitContent(margin: 5, offsetContent: false); // Explicit call to resize

            // Act
            string svg = document.Export();
            Console.WriteLine(svg); // let's log this for easier debug

            // Assert - width/height should be 120 + 2 * (5 + 0.5) = 131.0
            Assert.IsTrue(svg.Contains("width=\"131.00mm\""));
            Assert.IsTrue(svg.Contains("height=\"131.00mm\""));
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 131.00 131.00\""));

            // Confirm edge presence
            Assert.IsTrue(svg.Contains("L 130 10"), "Expected right edge not present");
            Assert.IsTrue(svg.Contains("L 130 130"), "Expected bottom-right corner not present");
            Assert.IsTrue(svg.Contains("L 10 130"), "Expected bottom edge not present");
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
            svg.Add(compoundShape, copy: true);

            svg.ResizeToFitContent(5, true);
            string exportedSvg = svg.Export();

            File.WriteAllText("advanced_shape.svg", exportedSvg);

            const string expected = """
                <svg xmlns="http://www.w3.org/2000/svg" width="170.10mm" height="60.10mm" viewBox="0 0 170.10 60.10">
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
