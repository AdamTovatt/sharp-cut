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
        public List<Edge> Edges { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="edges">The list of edges that define the shape.</param>
        public Shape(List<Edge> edges)
        {
            Edges = edges;
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
        /// Returns a string representation of the shape.
        /// </summary>
        /// <returns>A string listing all edges.</returns>
        public override string ToString()
        {
            return $"Shape with {Edges.Count} edges:\n" + string.Join("\n", Edges);
        }
    }
}
