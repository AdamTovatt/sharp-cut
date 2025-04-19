namespace SharpCut.Models
{
    /// <summary>
    /// Represents an axis-aligned rectangle shape defined by its top-left corner and size.
    /// </summary>
    public class Rectangle : IShape
    {
        /// <summary>
        /// The X coordinate of the top-left corner.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// The Y coordinate of the top-left corner.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="x">The X coordinate of the top-left corner.</param>
        /// <param name="y">The Y coordinate of the top-left corner.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Returns the edges of the rectangle as a list of <see cref="Edge"/> instances.
        /// </summary>
        /// <returns>List of four edges representing the rectangle.</returns>
        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();

            Point topLeft = new Point(X, Y);
            Point topRight = new Point(X + Width, Y);
            Point bottomRight = new Point(X + Width, Y + Height);
            Point bottomLeft = new Point(X, Y + Height);

            edges.Add(new Edge(topLeft, topRight));        // Top
            edges.Add(new Edge(topRight, bottomRight));    // Right
            edges.Add(new Edge(bottomRight, bottomLeft));  // Bottom
            edges.Add(new Edge(bottomLeft, topLeft));      // Left

            return edges;
        }

        /// <summary>
        /// Creates a rectangle shape from a specified origin point using the provided anchor type.
        /// </summary>
        /// <param name="origin">The reference point to anchor the rectangle from.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="anchor">The origin corner the rectangle should be anchored to.</param>
        /// <returns>A new instance of <see cref="Rectangle"/>.</returns>
        public static Rectangle CreateFromOrigin(float x, float y, float width, float height, Origin anchor)
        {
            switch (anchor)
            {
                case Origin.TopLeft:
                    break;
                case Origin.TopRight:
                    x -= width;
                    break;
                case Origin.BottomLeft:
                    y -= height;
                    break;
                case Origin.BottomRight:
                    x -= width;
                    y -= height;
                    break;
            }

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Returns a string representation of the rectangle.
        /// </summary>
        /// <returns>A string describing the rectangle.</returns>
        public override string ToString()
        {
            return $"Rectangle: ({X}, {Y}) {Width}x{Height}";
        }
    }
}