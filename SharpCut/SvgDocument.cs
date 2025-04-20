using System.Globalization;
using System.Runtime.Intrinsics.X86;
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
        public float Width { get; private set; }

        /// <summary>
        /// The height of the SVG canvas.
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// The stroke width used for drawing edges.
        /// </summary>
        public float StrokeWidth { get; set; }

        /// <summary>
        /// The stroke color used for drawing paths (in any valid CSS/SVG color format).
        /// </summary>
        public string StrokeColor { get; set; } = "black";

        /// <summary>
        /// The unit used for SVG dimensions (e.g. "mm", "cm", "in").
        /// </summary>
        public string Unit { get; set; } = "mm";

        /// <summary>
        /// Gets the shapes inside this SVG canvas.
        /// </summary>
        public IReadOnlyList<Shape> Shapes => _shapes;

        private readonly List<Shape> _shapes;
        private bool _autoResizeOnExport;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class with specified canvas dimensions,
        /// stroke width, stroke color, and unit for dimensions.
        /// </summary>
        /// <param name="width">The width of the canvas.</param>
        /// <param name="height">The height of the canvas.</param>
        /// <param name="strokeWidth">The stroke width for the paths.</param>
        /// <param name="strokeColor">The stroke color used for paths (default is "black").</param>
        /// <param name="unit">The unit used for the SVG width and height attributes (e.g. "mm", "in").</param>
        public SvgDocument(float width, float height, float strokeWidth = 1, string strokeColor = "black", string unit = "mm")
        {
            Width = width;
            Height = height;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor;
            Unit = unit;
            _shapes = new List<Shape>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class with auto-sizing behavior,
        /// stroke width, stroke color, and unit for dimensions.
        /// </summary>
        /// <param name="autoResizeOnExport">If true, the document will auto-resize to fit the content on export. Use copies to avoid modifying original shapes.</param>
        /// <param name="strokeWidth">The stroke width for the paths.</param>
        /// <param name="strokeColor">The stroke color used for paths (default is "black").</param>
        /// <param name="unit">The unit used for the SVG width and height attributes (e.g. "mm", "in").</param>
        public SvgDocument(bool autoResizeOnExport = true, float strokeWidth = 1, string strokeColor = "black", string unit = "mm")
        {
            _autoResizeOnExport = autoResizeOnExport;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor;
            Unit = unit;
            _shapes = new List<Shape>();
        }

        /// <summary>
        /// Adds a shape to the SVG document.
        /// Optionally adds a deep copy to prevent mutation side effects.
        /// </summary>
        /// <param name="shape">The shape to add.</param>
        /// <param name="copy">If true, adds a deep copy of the shape; otherwise, adds the original reference.</param>
        public void Add(Shape shape, bool copy = false)
        {
            _shapes.Add(copy ? shape.Copy() : shape);
        }

        /// <summary>
        /// Adds a shape to the SVG document.
        /// Optionally adds a deep copy to prevent mutation side effects.
        /// </summary>
        /// <param name="shape">The shape to add.</param>
        /// <param name="copy">If true, adds a deep copy of the shape; otherwise, adds the original reference.</param>
        public void Add(IShape shape, bool copy = false)
        {
            Shape newShapeInstance = new Shape(shape);
            _shapes.Add(copy ? newShapeInstance.Copy() : newShapeInstance);
        }

        /// <summary>
        /// Adds multiple shapes to the SVG document.
        /// Optionally adds deep copies to prevent mutation side effects.
        /// </summary>
        /// <param name="shapes">The list of shapes to add.</param>
        /// <param name="copy">If true, adds deep copies of the shapes; otherwise, adds original references.</param>
        public void Add(IEnumerable<Shape> shapes, bool copy = false)
        {
            foreach (Shape shape in shapes)
            {
                Add(shape, copy);
            }
        }

        /// <summary>
        /// Adds multiple IShape instances to the SVG document.
        /// Optionally adds deep copies to prevent mutation side effects.
        /// </summary>
        /// <param name="shapes">The list of <see cref="IShape"/> instances to add.</param>
        /// <param name="copy"> If true, adds deep copies of the shapes; otherwise, adds original references.</param>
        public void Add(IEnumerable<IShape> shapes, bool copy = false)
        {
            foreach (IShape shape in shapes)
            {
                Add(shape, copy);
            }
        }

        /// <summary>
        /// Resizes the document's width and height to tightly fit all added shapes, with the specified margin.
        /// Optionally offsets all shape edges so content starts at (margin, margin).
        /// 
        /// ⚠️ Note: If <paramref name="offsetContent"/> is true, this modifies the edge list of the shape instances in-place.
        /// If the same shape instance is reused elsewhere, those references will reflect the offset.
        /// Use <c>copy: true</c> when adding the shape to avoid unintended side effects.
        /// </summary>
        /// <param name="margin">Margin to add around the bounding box, in the same units as the shape coordinates.</param>
        /// <param name="offsetContent">If true, offsets all shape edges so content starts at (margin, margin).</param>
        public void ResizeToFitContent(float margin, bool offsetContent = false)
        {
            if (_shapes.Count == 0)
            {
                Width = 0;
                Height = 0;
                return;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (Shape shape in _shapes)
            {
                foreach (Edge edge in shape.Edges)
                {
                    Point[] points = { edge.Start, edge.End };

                    foreach (Point point in points)
                    {
                        if (point.X < minX) minX = point.X;
                        if (point.Y < minY) minY = point.Y;
                        if (point.X > maxX) maxX = point.X;
                        if (point.Y > maxY) maxY = point.Y;
                    }
                }
            }

            float contentWidth = maxX - minX;
            float contentHeight = maxY - minY;

            Width = contentWidth + 2 * margin;
            Height = contentHeight + 2 * margin;

            if (offsetContent)
            {
                float offsetX = margin - minX;
                float offsetY = margin - minY;

                for (int i = 0; i < _shapes.Count; i++)
                {
                    List<Edge> newEdges = new List<Edge>();

                    foreach (Edge edge in _shapes[i].Edges)
                    {
                        Point start = new Point(edge.Start.X + offsetX, edge.Start.Y + offsetY);
                        Point end = new Point(edge.End.X + offsetX, edge.End.Y + offsetY);
                        newEdges.Add(new Edge(start, end));
                    }

                    _shapes[i].Edges = newEdges;
                }
            }
        }

        /// <summary>
        /// Builds and returns the complete SVG markup string using the configured dimensions, stroke style, and unit.
        /// Shapes are exported as closed paths using 'M', 'L', and 'Z' commands.
        /// </summary>
        /// <returns>The SVG document as a formatted string.</returns>
        public string Export()
        {
            if (_autoResizeOnExport) ResizeToFitContent(5);

            StringBuilder builder = new StringBuilder();

            string widthStr = Width.ToString("0.00", CultureInfo.InvariantCulture) + Unit;
            string heightStr = Height.ToString("0.00", CultureInfo.InvariantCulture) + Unit;
            string viewBox = $"0 0 {Width.ToString(CultureInfo.InvariantCulture)} {Height.ToString(CultureInfo.InvariantCulture)}";
            string strokeStr = StrokeWidth.ToString(CultureInfo.InvariantCulture);

            builder.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{widthStr}\" height=\"{heightStr}\" viewBox=\"{viewBox}\">");
            builder.AppendLine($"<g fill=\"none\" stroke=\"{StrokeColor}\" stroke-width=\"{strokeStr}\">");

            foreach (Shape shape in _shapes)
            {
                List<List<Point>> paths = shape.GetClosedPaths();

                foreach (List<Point> path in paths)
                {
                    if (path.Count == 0)
                        continue;

                    StringBuilder d = new StringBuilder();
                    d.Append("M ");
                    d.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}", path[0].X, path[0].Y);

                    for (int i = 1; i < path.Count; i++)
                    {
                        d.AppendFormat(CultureInfo.InvariantCulture, " L {0} {1}", path[i].X, path[i].Y);
                    }

                    d.Append(" Z");
                    builder.AppendLine($"<path d=\"{d}\" />");
                }
            }

            builder.AppendLine("</g>");
            builder.AppendLine("</svg>");

            return builder.ToString();
        }
    }
}
