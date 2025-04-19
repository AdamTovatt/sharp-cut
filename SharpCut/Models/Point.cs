namespace SharpCut.Models
{
    /// <summary>
    /// Represents a 2D point with floating-point coordinates.
    /// </summary>
    public readonly struct Point
    {
        /// <summary>
        /// The X coordinate of the point.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// The Y coordinate of the point.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a string representation of the point.
        /// </summary>
        /// <returns>A string in the format "(X, Y)".</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
