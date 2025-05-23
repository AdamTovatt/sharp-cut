using System.Globalization;
using System.Text;
using System.Xml;
using SharpCut.Helpers;
using SharpCut.Models;

namespace SharpCut
{
    /// <summary>
    /// Represents an SVG document that can contain multiple shapes and export them as SVG markup.
    /// </summary>
    public class SvgDocument
    {
        /// <summary>
        /// The default unit of an SVG document.
        /// </summary>
        public const string DefaultUnit = "mm";

        /// <summary>
        /// The default color for the strokes in a SVG document.
        /// </summary>
        public const string DefaultColor = "black";

        /// <summary>
        /// The default stroke width of an SVG document.
        /// </summary>
        public const float DefaultStrokeWidth = 1.0f;

        /// <summary>
        /// The attributes of the document.
        /// </summary>
        public SvgDocumentAttributes Attributes { get; set; }

        /// <summary>
        /// Gets the shapes inside this SVG canvas.
        /// </summary>
        public IReadOnlyList<Shape> Shapes => _shapes;

        private readonly List<Shape> _shapes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class with specified canvas dimensions,
        /// stroke width, stroke color, and unit for dimensions.
        /// </summary>
        /// <param name="width">The width of the canvas.</param>
        /// <param name="height">The height of the canvas.</param>
        /// <param name="strokeWidth">The stroke width for the paths.</param>
        /// <param name="strokeColor">The stroke color used for paths (default is "black").</param>
        /// <param name="unit">The unit used for the SVG width and height attributes (e.g. "mm", "in").</param>
        public SvgDocument(
            float width,
            float height,
            float strokeWidth = DefaultStrokeWidth,
            string strokeColor = DefaultColor,
            string unit = DefaultUnit)
        {
            Attributes = new SvgDocumentAttributes(width, height, strokeWidth, strokeColor, unit);
            _shapes = new List<Shape>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class with auto-sizing behavior,
        /// stroke width, stroke color, and unit for dimensions.
        /// </summary>
        /// <param name="strokeWidth">The stroke width for the paths.</param>
        /// <param name="strokeColor">The stroke color used for paths (default is "black").</param>
        /// <param name="unit">The unit used for the SVG width and height attributes (e.g. "mm", "in").</param>
        public SvgDocument(
            float strokeWidth = DefaultStrokeWidth,
            string strokeColor = DefaultColor,
            string unit = DefaultUnit)
        {
            Attributes = new SvgDocumentAttributes(0, 0, strokeWidth, strokeColor, unit);
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
                Attributes.Width = 0;
                Attributes.Height = 0;
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

            float padding = margin + Attributes.StrokeWidth / 2f;

            float contentWidth = maxX - minX;
            float contentHeight = maxY - minY;

            Attributes.Width = contentWidth + 2 * padding;
            Attributes.Height = contentHeight + 2 * padding;

            if (offsetContent)
            {
                float offsetX = padding - minX;
                float offsetY = padding - minY;

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
            if (Attributes.Width == 0 && Attributes.Height == 0)
                throw new InvalidOperationException($"Export with both width and height of the document set to 0 is not allowed. \nCall the {nameof(ResizeToFitContent)}() method on the {nameof(SvgDocument)}-instance before exporting if you don't want to set the size yourself!");

            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" {Attributes}>");
            builder.AppendLine($"<g fill=\"none\" {Attributes.GetStrokeAttributes()}>");

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

        /// <summary>
        /// Will create an in memory instance of the <see cref="SvgDocument"/> from an <see cref="XmlReader"/> that is reading SVG data in string format.
        /// </summary>
        /// <param name="documentReader">The <see cref="XmlReader"/> that can read SVG data in string format.</param>
        public static SvgDocument Import(XmlReader documentReader)
        {
            float documentWidth = 0;
            float documentHeight = 0;
            string? documentUnit = null;
            float strokeWidth = DefaultStrokeWidth;
            string strokeColor = DefaultColor;

            List<IShape> shapes = new List<IShape>();

            while (documentReader.Read())
            {
                if (documentReader.NodeType == XmlNodeType.EndElement || documentReader.Name == string.Empty)
                    continue;

                switch (documentReader.Name)
                {
                    case "svg":
                        HandleSvgElement(documentReader, ref documentWidth, ref documentHeight, ref documentUnit);
                        break;
                    case "path":
                        HandlePathElement(documentReader, shapes);
                        break;
                    case "g":
                        HandleGElement(documentReader, ref strokeWidth, ref strokeColor);
                        break;
                    default:
                        break;
                }
            }

            SvgDocument result = new SvgDocument(
                width: documentWidth,
                height: documentHeight,
                strokeWidth: strokeWidth,
                strokeColor: strokeColor,
                unit: documentUnit ?? DefaultUnit);

            result.Add(shapes);

            return result;
        }

        private static void HandleGElement(XmlReader documentReader, ref float strokeWidth, ref string strokeColor)
        {
            string? color = documentReader.GetAttribute("stroke");
            string? width = documentReader.GetAttribute("stroke-width");

            strokeColor = color ?? DefaultColor;

            if (width == null)
                strokeWidth = DefaultStrokeWidth;
            else
                strokeWidth = float.Parse(width, CultureInfo.InvariantCulture);
        }

        private static void HandlePathElement(
            XmlReader documentReader,
            List<IShape> shapes)
        {
            string pathData = documentReader.GetAttribute("d") ??
                throw new InvalidDataException($"Missing path data");

            using (PathReader pathReader = new PathReader(pathData))
            {
                pathReader.ReadStartOfPath();

                List<Point> points = new List<Point>();
                bool didReadCloseCharacter = false;

                while (true)
                {
                    Point point = pathReader.ReadPoint();
                    points.Add(point);

                    int next = pathReader.Read(); // move on to the next character

                    if (next == ' ') // there was a space, check the character after that
                        next = pathReader.Read(); // get the character after the point

                    if (next == 'L')
                    {
                        pathReader.Read();
                    }
                    else if (next == 'Z')
                    {
                        didReadCloseCharacter = true;
                        break;
                    }
                    else if (next == -1)
                        break;
                    else
                        throw new InvalidDataException($"Unexpected character '{(char)next}' in path.");
                }

                Shape shape = Shape.FromPoints(points, didReadCloseCharacter);

                shapes.Add(shape);
            }
        }

        private static void HandleSvgElement(
            XmlReader documentReader,
            ref float documentWidth,
            ref float documentHeight,
            ref string? documentUnit)
        {
            if (documentReader.GetAttribute("width") is string width)
            {
                Scalar parsedWidth = Scalar.FromString(width);
                documentWidth = parsedWidth.Value;
                documentUnit = parsedWidth.Unit;
            }

            if (documentReader.GetAttribute("height") is string height)
            {
                Scalar parsedHeight = Scalar.FromString(height);
                documentHeight = parsedHeight.Value;
                documentUnit ??= parsedHeight.Unit;
            }
        }

        /// <summary>
        /// Will create an in memory instance of the <see cref="SvgDocument"/> from a string containing SVG data.
        /// </summary>
        /// <param name="svgDocumentString">The string containing the SVG data.</param>
        public static SvgDocument Import(string svgDocumentString)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(svgDocumentString)))
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.DtdProcessing = DtdProcessing.Parse;

                using (XmlReader xmlReader = XmlReader.Create(memoryStream, readerSettings))
                {
                    return Import(xmlReader);
                }
            }
        }

        /// <summary>
        /// Will create an in memory instance of the <see cref="SvgDocument"/> from a <see cref="Stream"/> containing readable SVG data in string format.
        /// </summary>
        /// <param name="svgDocumentStream">The stream containing SVG data in string format.</param>
        public static SvgDocument Import(Stream svgDocumentStream)
        {
            using (XmlReader xmlReader = XmlReader.Create(svgDocumentStream))
            {
                return Import(xmlReader);
            }
        }
    }
}
