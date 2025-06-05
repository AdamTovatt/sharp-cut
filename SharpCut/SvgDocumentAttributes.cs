using SharpCut.Models;
using System.Globalization;

namespace SharpCut
{
    /// <summary>
    /// Attributes of an <see cref="SvgDocument"/>. Contains things like <see cref="Width"/>, <see cref="Height"/>, <see cref="Unit"/>, <see cref="StrokeWidth"/> and <see cref="StrokeColor"/>.
    /// </summary>
    public class SvgDocumentAttributes
    {
        /// <summary>
        /// Represents the SVG viewBox attribute, defining the internal coordinate system of the SVG canvas.
        /// </summary>
        public class ViewBox
        {
            /// <summary>
            /// The minimum X coordinate of the viewBox.
            /// </summary>
            public float MinX { get; set; }

            /// <summary>
            /// The minimum Y coordinate of the viewBox.
            /// </summary>
            public float MinY { get; set; }

            /// <summary>
            /// The width of the internal coordinate system.
            /// </summary>
            public float Width { get; set; }

            /// <summary>
            /// The height of the internal coordinate system.
            /// </summary>
            public float Height { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewBox"/> class with specified coordinates and size.
            /// </summary>
            /// <param name="minX">Minimum X coordinate.</param>
            /// <param name="minY">Minimum Y coordinate.</param>
            /// <param name="width">Width of the viewBox.</param>
            /// <param name="height">Height of the viewBox.</param>
            public ViewBox(float minX, float minY, float width, float height)
            {
                MinX = minX;
                MinY = minY;
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Returns the string representation of the viewBox, suitable for SVG output.
            /// </summary>
            /// <returns>A space-separated string in the format "minX minY width height".</returns>
            public override string ToString()
            {
                return $"{MinX.ToString(CultureInfo.InvariantCulture)} " +
                       $"{MinY.ToString(CultureInfo.InvariantCulture)} " +
                       $"{Width.ToString(CultureInfo.InvariantCulture)} " +
                       $"{Height.ToString(CultureInfo.InvariantCulture)}";
            }
        }

        /// <summary>
        /// The default color of the stroke for an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public const string DefaultStrokeColor = "black";

        /// <summary>
        /// The default unit for an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public const string DefaultUnit = "mm";

        /// <summary>
        /// The physical width of the SVG canvas.
        /// </summary>
        public float Width { get; internal set; }

        /// <summary>
        /// The physical height of the SVG canvas.
        /// </summary>
        public float Height { get; internal set; }

        /// <summary>
        /// The stroke width used for drawing edges.
        /// </summary>
        public float StrokeWidth { get; set; }

        /// <summary>
        /// The stroke color used for drawing paths (in any valid CSS/SVG color format).
        /// </summary>
        public string StrokeColor { get; set; } = DefaultStrokeColor;

        /// <summary>
        /// The unit used for SVG dimensions (e.g. "mm", "cm", "in").
        /// </summary>
        public string Unit { get; set; } = DefaultUnit;

        /// <summary>
        /// The viewBox defining the internal coordinate system.
        /// </summary>
        public ViewBox ViewBoxDimensions { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public SvgDocumentAttributes(
            float width,
            float height,
            float strokeWidth,
            string strokeColor = DefaultStrokeColor,
            string unit = DefaultUnit,
            ViewBox? viewBox = null)
        {
            Width = width;
            Height = height;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor;
            Unit = unit;
            ViewBoxDimensions = viewBox ?? new ViewBox(0, 0, width, height);
        }

        /// <summary>
        /// Returns a string with the width, height and view box attributes that can be used as the attributes for the svg-xml element.
        /// </summary>
        public override string ToString()
        {
            Scalar width = new Scalar(Width, Unit);
            Scalar height = new Scalar(Height, Unit);
            string viewBox = ViewBoxDimensions.ToString();

            return $"width=\"{width}\" height=\"{height}\" viewBox=\"{viewBox}\"";
        }

        /// <summary>
        /// Returns a string with the stroke color and stroke width attributes that can be used as the attributes for an element inside the svg-xml element.
        /// </summary>
        public string GetStrokeAttributes()
        {
            string strokeWidth = StrokeWidth.ToString(CultureInfo.InvariantCulture);
            return $"stroke=\"{StrokeColor}\" stroke-width=\"{strokeWidth}\"";
        }
    }
}
