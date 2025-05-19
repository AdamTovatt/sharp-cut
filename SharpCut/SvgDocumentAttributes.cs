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
        /// The default color of the stroke for an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public const string DefaultStrokeColor = "black";

        /// <summary>
        /// The default unit for an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public const string DefaultUnit = "mm";

        /// <summary>
        /// The width of the SVG canvas.
        /// </summary>
        public float Width { get; internal set; }

        /// <summary>
        /// The height of the SVG canvas.
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
        /// Creates an instance of <see cref="SvgDocumentAttributes"/>.
        /// </summary>
        public SvgDocumentAttributes(
            float width,
            float height,
            float strokeWidth,
            string strokeColor = DefaultStrokeColor,
            string unit = DefaultUnit)
        {
            Width = width;
            Height = height;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor;
            Unit = unit;
        }

        /// <summary>
        /// Returns a string with the width, height and view box attributes that can be used as the attributes for the svg-xml element.
        /// </summary>
        public override string ToString()
        {
            Scalar width = new Scalar(Width, Unit);
            Scalar height = new Scalar(Height, Unit);
            string viewBox = $"0 0 {width.GetValueAsString()} {height.GetValueAsString()}";

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
