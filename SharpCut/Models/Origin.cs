namespace SharpCut.Models
{
    /// <summary>
    /// Specifies the anchor corner to use when creating a rectangle from an origin point.
    /// </summary>
    public enum Origin
    {
        /// <summary>
        /// The origin represents the top-left corner of the rectangle.
        /// </summary>
        TopLeft,

        /// <summary>
        /// The origin represents the top-right corner of the rectangle.
        /// </summary>
        TopRight,

        /// <summary>
        /// The origin represents the bottom-left corner of the rectangle.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The origin represents the bottom-right corner of the rectangle.
        /// </summary>
        BottomRight
    }
}