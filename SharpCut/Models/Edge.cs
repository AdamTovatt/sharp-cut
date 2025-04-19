namespace SharpCut.Models
{
    /// <summary>
    /// Represents a straight edge (line segment) between two points.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// The start point of the edge.
        /// </summary>
        public Point Start { get; }

        /// <summary>
        /// The end point of the edge.
        /// </summary>
        public Point End { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="start">The start point of the edge.</param>
        /// <param name="end">The end point of the edge.</param>
        public Edge(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Creates a new edge with the same start and end points.
        /// </summary>
        /// <returns>A copy of this edge.</returns>
        public Edge Copy()
        {
            return new Edge(new Point(Start.X, Start.Y), new Point(End.X, End.Y));
        }

        /// <summary>
        /// Subtracts the overlapping portion of another colinear edge from this edge.
        /// </summary>
        /// <param name="other">The edge to subtract.</param>
        /// <returns>A list of edges representing the remaining (non-overlapping) parts.</returns>
        public List<Edge> Subtract(Edge other)
        {
            List<Edge> result = new List<Edge>();

            bool isHorizontal = Start.Y == End.Y && other.Start.Y == other.End.Y;
            bool isVertical = Start.X == End.X && other.Start.X == other.End.X;

            if (!isHorizontal && !isVertical)
            {
                result.Add(this);
                return result;
            }

            if (isHorizontal && Start.Y != other.Start.Y)
            {
                result.Add(this);
                return result;
            }

            if (isVertical && Start.X != other.Start.X)
            {
                result.Add(this);
                return result;
            }

            Point a1, a2;
            Point b1, b2;

            if (isHorizontal)
            {
                a1 = Start.X <= End.X ? Start : End;
                a2 = Start.X <= End.X ? End : Start;

                b1 = other.Start.X <= other.End.X ? other.Start : other.End;
                b2 = other.Start.X <= other.End.X ? other.End : other.Start;
            }
            else
            {
                a1 = Start.Y <= End.Y ? Start : End;
                a2 = Start.Y <= End.Y ? End : Start;

                b1 = other.Start.Y <= other.End.Y ? other.Start : other.End;
                b2 = other.Start.Y <= other.End.Y ? other.End : other.Start;
            }

            float aStart = isHorizontal ? a1.X : a1.Y;
            float aEnd = isHorizontal ? a2.X : a2.Y;
            float bStart = isHorizontal ? b1.X : b1.Y;
            float bEnd = isHorizontal ? b2.X : b2.Y;

            if (bEnd <= aStart || bStart >= aEnd)
            {
                result.Add(this);
                return result;
            }

            float overlapStart = Math.Max(aStart, bStart);
            float overlapEnd = Math.Min(aEnd, bEnd);

            if (overlapStart > aStart)
            {
                Point start = isHorizontal ? new Point(aStart, a1.Y) : new Point(a1.X, aStart);
                Point end = isHorizontal ? new Point(overlapStart, a1.Y) : new Point(a1.X, overlapStart);
                result.Add(new Edge(start, end));
            }

            if (overlapEnd < aEnd)
            {
                Point start = isHorizontal ? new Point(overlapEnd, a1.Y) : new Point(a1.X, overlapEnd);
                Point end = isHorizontal ? new Point(aEnd, a1.Y) : new Point(a1.X, aEnd);
                result.Add(new Edge(start, end));
            }

            return result;
        }

        /// <summary>
        /// Returns a list of points evenly distributed along this edge,
        /// optionally with margins from the start and end, and control over endpoint inclusion.
        /// </summary>
        /// <param name="count">The number of points to generate.</param>
        /// <param name="startMargin">The distance from the start of the edge to the first point.</param>
        /// <param name="endMargin">The distance from the end of the edge to the last point.</param>
        /// <param name="includeEndpoints">
        /// If true, points include the start and end positions of the usable segment.
        /// If false, points are spaced between the margins only.
        /// </param>
        /// <returns>A list of <see cref="Point"/> values along the edge.</returns>
        public List<Point> GetDistributedPoints(int count, float startMargin = 0, float endMargin = 0, bool includeEndpoints = false)
        {
            List<Point> points = new List<Point>();

            if (count <= 0)
            {
                return points;
            }

            float dx = End.X - Start.X;
            float dy = End.Y - Start.Y;

            float totalLength = (float)Math.Sqrt(dx * dx + dy * dy);
            float usableLength = totalLength - startMargin - endMargin;

            if (usableLength <= 0 || count == 1)
            {
                float ratio = startMargin + usableLength / 2f;
                float x = Start.X + dx * (ratio / totalLength);
                float y = Start.Y + dy * (ratio / totalLength);
                points.Add(new Point(x, y));
                return points;
            }

            for (int i = 0; i < count; i++)
            {
                float t = includeEndpoints
                    ? (float)i / (count - 1)                          // includes ends
                    : (float)(i + 1) / (count + 1);                   // excludes ends

                float ratio = startMargin + t * usableLength;
                float x = Start.X + dx * (ratio / totalLength);
                float y = Start.Y + dy * (ratio / totalLength);
                points.Add(new Point(x, y));
            }

            return points;
        }

        /// <summary>
        /// Returns a string representation of the edge.
        /// </summary>
        /// <returns>A string describing the edge's start and end points.</returns>
        public override string ToString()
        {
            return $"Edge: {Start} -> {End}";
        }
    }
}
