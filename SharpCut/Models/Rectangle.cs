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
        public float X { get; set; }

        /// <summary>
        /// The Y coordinate of the top-left corner.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height { get; set; }

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
        /// <param name="x">The x position to place the rectangle at.</param>
        /// <param name="y">The y position to place the rectangle at.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="anchor">The origin position the rectangle should be anchored to.</param>
        /// <returns>A new instance of <see cref="Rectangle"/>.</returns>
        public static Rectangle CreateFromOrigin(float x, float y, float width, float height, Origin anchor)
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            rect.SetPosition(x, y, anchor);
            return rect;
        }

        /// <summary>
        /// Returns the edge of the rectangle that corresponds to the given side.
        /// </summary>
        /// <param name="side">The side of the rectangle to retrieve.</param>
        /// <returns>An <see cref="Edge"/> representing the specified side.</returns>
        public Edge GetEdge(Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return new Edge(new Point(X, Y), new Point(X + Width, Y));

                case Side.Right:
                    return new Edge(new Point(X + Width, Y), new Point(X + Width, Y + Height));

                case Side.Bottom:
                    return new Edge(new Point(X + Width, Y + Height), new Point(X, Y + Height));

                case Side.Left:
                    return new Edge(new Point(X, Y + Height), new Point(X, Y));

                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        /// <summary>
        /// Sets the position of the rectangle so that the specified origin point aligns with the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate to align to.</param>
        /// <param name="y">The y-coordinate to align to.</param>
        /// <param name="origin">The origin point of the rectangle to align.</param>
        public Rectangle SetPosition(float x, float y, Origin origin)
        {
            switch (origin)
            {
                case Origin.TopLeft:
                    X = x;
                    Y = y;
                    break;

                case Origin.TopCenter:
                    X = x - Width / 2;
                    Y = y;
                    break;

                case Origin.TopRight:
                    X = x - Width;
                    Y = y;
                    break;

                case Origin.CenterLeft:
                    X = x;
                    Y = y - Height / 2;
                    break;

                case Origin.Center:
                    X = x - Width / 2;
                    Y = y - Height / 2;
                    break;

                case Origin.CenterRight:
                    X = x - Width;
                    Y = y - Height / 2;
                    break;

                case Origin.BottomLeft:
                    X = x;
                    Y = y - Height;
                    break;

                case Origin.BottomCenter:
                    X = x - Width / 2;
                    Y = y - Height;
                    break;

                case Origin.BottomRight:
                    X = x - Width;
                    Y = y - Height;
                    break;
            }

            return this;
        }

        /// <summary>
        /// Moves the rectangle by the specified horizontal and vertical offsets.
        /// </summary>
        /// <param name="dx">The amount to move along the X axis.</param>
        /// <param name="dy">The amount to move along the Y axis.</param>
        /// <returns>The same rectangle instance, for chaining.</returns>
        public Rectangle Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
            return this;
        }

        /// <summary>
        /// Creates a list of new rectangles by placing copies of this rectangle at the specified points,
        /// aligned according to the given origin.
        /// </summary>
        /// <param name="points">The points where copies should be placed.</param>
        /// <param name="origin">The anchor point of this rectangle to align with each target point.</param>
        /// <returns>A list of new <see cref="Rectangle"/> instances positioned at the given points.</returns>
        public List<Rectangle> PlaceCopiesOnPoints(IEnumerable<Point> points, Origin origin)
        {
            List<Rectangle> result = new List<Rectangle>();

            foreach (Point point in points)
            {
                Rectangle copy = new Rectangle(X, Y, Width, Height);
                copy.SetPosition(point.X, point.Y, origin);
                result.Add(copy);
            }

            return result;
        }

        /// <summary>
        /// Returns a shape representation of the rectangle.
        /// </summary>
        /// <returns>A shape that has the same edges as this rectangle.</returns>
        public Shape ToShape()
        {
            return new Shape(this);
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