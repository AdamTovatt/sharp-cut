using System.Globalization;
using System.Text;
using SharpCut.Models;

namespace SharpCut
{
    /// <summary>
    /// Represents an SVG document that can contain multiple shapes and export them as SVG markup.
    /// </summary>
    public class SvgDocument
    {
        /// <summary>
        /// The width of the SVG canvas.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// The height of the SVG canvas.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// The stroke width used for drawing edges.
        /// </summary>
        public float StrokeWidth { get; }

        private readonly List<Shape> _shapes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class with specified canvas dimensions and stroke width.
        /// </summary>
        /// <param name="width">The width of the canvas.</param>
        /// <param name="height">The height of the canvas.</param>
        /// <param name="strokeWidth">The stroke width for the paths.</param>
        public SvgDocument(float width, float height, float strokeWidth = 1)
        {
            Width = width;
            Height = height;
            StrokeWidth = strokeWidth;
            _shapes = new List<Shape>();
        }

        /// <summary>
        /// Adds a shape to the SVG document.
        /// </summary>
        /// <param name="shape">The shape to add.</param>
        public void AddShape(Shape shape)
        {
            _shapes.Add(shape);
        }

        /// <summary>
        /// Builds and returns the complete SVG markup string.
        /// </summary>
        /// <returns>The SVG document as a string.</returns>
        public string Export()
        {
            StringBuilder builder = new StringBuilder();

            string widthStr = Width.ToString(CultureInfo.InvariantCulture);
            string heightStr = Height.ToString(CultureInfo.InvariantCulture);
            string strokeStr = StrokeWidth.ToString(CultureInfo.InvariantCulture);

            builder.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{widthStr}\" height=\"{heightStr}\" viewBox=\"0 0 {widthStr} {heightStr}\">");
            builder.AppendLine($"<g fill=\"none\" stroke=\"black\" stroke-width=\"{strokeStr}\">");

            foreach (Shape shape in _shapes)
            {
                foreach (Edge edge in shape.Edges)
                {
                    string x1 = edge.Start.X.ToString(CultureInfo.InvariantCulture);
                    string y1 = edge.Start.Y.ToString(CultureInfo.InvariantCulture);
                    string x2 = edge.End.X.ToString(CultureInfo.InvariantCulture);
                    string y2 = edge.End.Y.ToString(CultureInfo.InvariantCulture);

                    builder.AppendLine($"<path d=\"M {x1} {y1} L {x2} {y2}\" />");
                }
            }

            builder.AppendLine("</g>");
            builder.AppendLine("</svg>");

            return builder.ToString();
        }
    }
}
