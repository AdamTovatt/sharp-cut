namespace SharpCut.Models
{
    /// <summary>
    /// Specifies the anchor point to use when creating a rectangle from an origin point.
    /// </summary>
    public enum Origin
    {
        /// <summary>The origin represents the top-left corner of the rectangle.</summary>
        TopLeft,

        /// <summary>The origin represents the top-center edge of the rectangle.</summary>
        TopCenter,

        /// <summary>The origin represents the top-right corner of the rectangle.</summary>
        TopRight,

        /// <summary>The origin represents the center-left edge of the rectangle.</summary>
        CenterLeft,

        /// <summary>The origin represents the center of the rectangle.</summary>
        Center,

        /// <summary>The origin represents the center-right edge of the rectangle.</summary>
        CenterRight,

        /// <summary>The origin represents the bottom-left corner of the rectangle.</summary>
        BottomLeft,

        /// <summary>The origin represents the bottom-center edge of the rectangle.</summary>
        BottomCenter,

        /// <summary>The origin represents the bottom-right corner of the rectangle.</summary>
        BottomRight
    }
}
