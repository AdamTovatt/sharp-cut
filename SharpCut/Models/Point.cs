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
        /// Adds two points by summing their X and Y components.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>A new <see cref="Point"/> representing the sum.</returns>
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Subtracts one point from another by subtracting their X and Y components.
        /// </summary>
        /// <param name="a">The first point (minuend).</param>
        /// <param name="b">The second point (subtrahend).</param>
        /// <returns>A new <see cref="Point"/> representing the difference.</returns>
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Determines whether two <see cref="Point"/> instances have the same coordinates.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>True if both points have equal X and Y values; otherwise, false.</returns>
        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Determines whether two <see cref="Point"/> instances have different coordinates.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>True if the points differ in X or Y; otherwise, false.</returns>
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the object is a <see cref="Point"/> with the same coordinates; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is Point other && this == other;
        }

        /// <summary>
        /// Returns a hash code for this point.
        /// </summary>
        /// <returns>A hash code based on the X and Y values.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
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
