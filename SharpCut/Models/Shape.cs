namespace SharpCut.Models
{
    /// <summary>
    /// Represents a finalized shape composed of edges.
    /// </summary>
    public class Shape : IShape
    {
        /// <summary>
        /// The edges that define the shape.
        /// </summary>
        public List<Edge> Edges { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="edges">The list of edges that define the shape.</param>
        public Shape(List<Edge> edges)
        {
            Edges = edges;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class from a IShape instance.
        /// </summary>
        /// <param name="shape">The IShape instance that contains the edges to use for creating a new shape.</param>
        public Shape(IShape shape)
        {
            Edges = shape.GetEdges();
        }

        /// <summary>
        /// Returns the edges of the shape.
        /// </summary>
        /// <returns>The list of edges.</returns>
        public List<Edge> GetEdges()
        {
            return Edges;
        }

        /// <summary>
        /// Returns all closed paths in this shape as ordered point loops.
        /// Each path is a sequence of points forming a closed polygon.
        /// </summary>
        /// <returns>A list of closed paths, where each path is a list of points.</returns>
        public List<List<Point>> GetClosedPaths()
        {
            List<List<Point>> paths = new List<List<Point>>();
            HashSet<Edge> unused = new HashSet<Edge>(Edges);
            Dictionary<Point, List<Edge>> edgeMap = new Dictionary<Point, List<Edge>>();

            foreach (Edge edge in Edges)
            {
                if (!edgeMap.ContainsKey(edge.Start))
                {
                    edgeMap[edge.Start] = new List<Edge>();
                }
                edgeMap[edge.Start].Add(edge);

                if (!edgeMap.ContainsKey(edge.End))
                {
                    edgeMap[edge.End] = new List<Edge>();
                }
                edgeMap[edge.End].Add(edge);
            }

            while (unused.Count > 0)
            {
                Edge startEdge = null!;
                foreach (Edge e in unused)
                {
                    startEdge = e;
                    break;
                }

                List<Point> path = new List<Point> { startEdge.Start };
                Point current = startEdge.End;
                unused.Remove(startEdge);

                while (current != path[0])
                {
                    path.Add(current);

                    if (!edgeMap.TryGetValue(current, out List<Edge>? nextEdges))
                    {
                        break;
                    }

                    Edge? next = nextEdges.Find(e => unused.Contains(e));
                    if (next == null)
                    {
                        break;
                    }

                    unused.Remove(next);

                    if (next.Start == current)
                    {
                        current = next.End;
                    }
                    else if (next.End == current)
                    {
                        current = next.Start;
                    }
                    else
                    {
                        break;
                    }
                }

                if (path.Count > 2 && path[0] == path[^1])
                {
                    path.RemoveAt(path.Count - 1);
                }

                paths.Add(path);
            }

            return paths;
        }

        /// <summary>
        /// Creates a deep copy of the shape and its edges.
        /// </summary>
        /// <returns>A new <see cref="Shape"/> instance with copied edges.</returns>
        public Shape Copy()
        {
            List<Edge> copiedEdges = new List<Edge>();

            foreach (Edge edge in Edges)
            {
                copiedEdges.Add(edge.Copy());
            }

            return new Shape(copiedEdges);
        }

        /// <summary>
        /// Returns a string representation of the shape.
        /// </summary>
        /// <returns>A string listing all edges.</returns>
        public override string ToString()
        {
            return $"Shape with {Edges.Count} edges:\n" + string.Join("\n", Edges);
        }
    }
}
