using System.Collections.Generic;

namespace SharpCut.Models
{
    /// <summary>
    /// Represents a shape composed of multiple child shapes.
    /// </summary>
    public class CompoundShape : IShape
    {
        private readonly List<IShape> _shapes = new List<IShape>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundShape"/> class with an empty shape list.
        /// </summary>
        public CompoundShape()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundShape"/> class with a single shape.
        /// </summary>
        /// <param name="shape">The initial shape to include.</param>
        public CompoundShape(IShape shape)
        {
            _shapes.Add(shape);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundShape"/> class with multiple shapes.
        /// </summary>
        /// <param name="shapes">The shapes to include in the compound.</param>
        public CompoundShape(IEnumerable<IShape> shapes)
        {
            _shapes.AddRange(shapes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundShape"/> class with one required shape followed by additional shapes.
        /// </summary>
        /// <param name="first">The first shape to include.</param>
        /// <param name="others">Additional shapes to include.</param>
        public CompoundShape(IShape first, params IShape[] others)
        {
            _shapes.Add(first);
            _shapes.AddRange(others);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundShape"/> class with one required shape followed by additional shapes.
        /// </summary>
        /// <param name="first">The first shape to include.</param>
        /// <param name="others">Additional shapes to include.</param>
        public CompoundShape(IShape first, IEnumerable<IShape> others)
        {
            _shapes.Add(first);
            _shapes.AddRange(others);
        }

        /// <summary>
        /// Adds a shape to the compound.
        /// </summary>
        /// <param name="shape">The shape to add.</param>
        public void Add(IShape shape)
        {
            _shapes.Add(shape);
        }

        /// <summary>
        /// Adds multiple shapes to the compound.
        /// </summary>
        /// <param name="shapes">The shapes to add.</param>
        public void Add(IEnumerable<IShape> shapes)
        {
            _shapes.AddRange(shapes);
        }

        /// <summary>
        /// Adds multiple shapes to the compound using a params array.
        /// </summary>
        /// <param name="shapes">The shapes to add.</param>
        public void Add(params IShape[] shapes)
        {
            _shapes.AddRange(shapes);
        }

        /// <summary>
        /// Returns the edges of the compound shape after removing overlapping edges symmetrically.
        /// </summary>
        /// <returns>A combined list of de-duplicated edges from all included shapes.</returns>
        public List<Edge> GetEdges()
        {
            Shape result = Builders.ShapeBuilder.Build(_shapes, symmetrical: true);
            return result.Edges;
        }

        /// <summary>
        /// Returns a string representation of the compound shape.
        /// </summary>
        /// <returns>A string summarizing the number of included shapes.</returns>
        public override string ToString()
        {
            return $"CompoundShape with {_shapes.Count} shape(s)";
        }
    }
}
