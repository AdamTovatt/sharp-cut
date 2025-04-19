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

            SvgDocument doc = new SvgDocument(100, 100);
            doc.AddShape(shape);

            string svg = doc.Export();

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

            SvgDocument doc = new SvgDocument(100, 100);
            doc.AddShape(shape1);
            doc.AddShape(shape2);

            string svg = doc.Export();

            // 8 lines total (4 from each rectangle)
            int pathCount = svg.Split("<path").Length - 1;
            Assert.AreEqual(8, pathCount);
        }

        [TestMethod]
        public void Export_UsesCorrectCanvasSize()
        {
            SvgDocument doc = new SvgDocument(123, 456);
            string svg = doc.Export();

            Assert.IsTrue(svg.Contains("width=\"123\""));
            Assert.IsTrue(svg.Contains("height=\"456\""));
            Assert.IsTrue(svg.Contains("viewBox=\"0 0 123 456\""));
        }

        [TestMethod]
        public void Export_RespectsStrokeWidth()
        {
            SvgDocument doc = new SvgDocument(100, 100, strokeWidth: 3.5f);
            string svg = doc.Export();

            Assert.IsTrue(svg.Contains("stroke-width=\"3.5\""));
        }

        [TestMethod]
        public void Export_BlockUFlipped_ProducesExpectedSvgMarkup()
        {
            Rectangle outer = new Rectangle(0, 0, 40, 60);
            Rectangle cutout = new Rectangle(10, 0, 20, 30); // Top-center cutout (appears bottom in SVG)

            List<IShape> shapes = new List<IShape> { outer, cutout };

            Shape finalShape = ShapeBuilder.Build(shapes, symmetrical: true);

            SvgDocument doc = new SvgDocument(50, 70);
            doc.AddShape(finalShape);

            string svg = doc.Export();

            string expected = """
            <svg xmlns="http://www.w3.org/2000/svg" width="50" height="70" viewBox="0 0 50 70">
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

            // Normalize for comparison
            string normalizedActual = svg.Replace("\r\n", "\n").Trim();
            string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

            Assert.AreEqual(normalizedExpected, normalizedActual);
        }
    }
}