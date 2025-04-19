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
