namespace SharpCut.Models
{
    /// <summary>
    /// Represents a geometric shape that can produce a set of edges.
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// Returns the edges that define the shape.
        /// </summary>
        /// <returns>A list of edges.</returns>
        List<Edge> GetEdges();
    }
}
