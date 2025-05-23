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
        /// Returns a <see cref="Shape"/> created from a list of points by creating an edge between each pair of points that follow each other in the list.
        /// </summary>
        /// <param name="points">The list of points to turn into a shape.</param>
        /// <param name="closePath">If set to true, the shape will be closed by taking the last and first point in the list and connceting them with an edge.</param>
        public static Shape FromPoints(List<Point> points, bool closePath)
        {
            List<Edge> edges = new List<Edge>(points.Count - 1);

            for (int i = 0; i < points.Count - 1; i++)
            {
                Edge edge = new Edge(points[i], points[i + 1]);
                edges.Add(edge);
            }

            if (points.Count > 1 && closePath)
                edges.Add(new Edge(points[0], points[points.Count - 1]));

            return new Shape(edges);
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

        /// <summary>
        /// Returns a boolean indiciating wether or not the other instance of <see cref="Shape"/> contains the same instances of <see cref="Edge"/> by value.
        /// This compares not by reference but rather by value of the start and end points of each <see cref="Edge"/> in each <see cref="Shape"/>.
        /// This method considers "duplicate" edges and does not incorrectly return true for shapes pairs where one of the shapes has to of an edge and the other only one of the same edge.
        /// </summary>
        /// <param name="otherShape">The other instance of <see cref="Shape"/> to compare to.</param>
        /// <returns>A boolean indicating wether or not the other <see cref="Shape"/> is the same as the one this method is called on by values of the start and end points of all edges.</returns>
        public bool IsSameShapeByValues(Shape otherShape)
        {
            int matchedEdges = 0;

            HashSet<Edge> consumedEdges = new HashSet<Edge>();

            foreach (Edge otherEdge in otherShape.Edges)
            {
                consumedEdges.Clear();
                bool wasFound = false;

                foreach (Edge localEdge in Edges)
                {
                    if (localEdge.IsSameEdgeByValues(otherEdge) && !consumedEdges.Contains(localEdge))
                    {
                        wasFound = true;
                        matchedEdges++;
                        consumedEdges.Add(localEdge);
                        break;
                    }
                }

                if (!wasFound)
                    return false;
            }

            return matchedEdges == Edges.Count;
        }
    }
}
