using System.Collections.Generic;
using SharpCut.Models;

namespace SharpCut.Builders
{
    /// <summary>
    /// Builds a shape from multiple input shapes by optionally removing overlapping edges symmetrically or in insertion order.
    /// </summary>
    public static class ShapeBuilder
    {
        /// <summary>
        /// Builds a shape from the given input shapes, with optional symmetrical edge de-duplication.
        /// </summary>
        /// <param name="shapes">The input shapes to process.</param>
        /// <param name="symmetrical">
        /// If true, overlapping edges are removed in both directions, producing a fully order-independent result.
        /// If false, shapes earlier in the list take priority over later ones.
        /// </param>
        /// <returns>A new <see cref="Shape"/> instance with cleaned edges.</returns>
        public static Shape Build(IList<IShape> shapes, bool symmetrical = false)
        {
            return symmetrical
                ? BuildSymmetrical(shapes)
                : BuildOrdered(shapes);
        }

        private static Shape BuildOrdered(IList<IShape> shapes)
        {
            List<Edge> allEdges = new List<Edge>();

            foreach (IShape shape in shapes)
            {
                List<Edge> edges = shape.GetEdges();

                foreach (Edge edge in edges)
                {
                    List<Edge> toAdd = new List<Edge> { edge };

                    for (int i = 0; i < allEdges.Count; i++)
                    {
                        List<Edge> updated = new List<Edge>();

                        foreach (Edge candidate in toAdd)
                        {
                            updated.AddRange(candidate.Subtract(allEdges[i]));
                        }

                        toAdd = updated;
                    }

                    allEdges.AddRange(toAdd);
                }
            }

            return new Shape(allEdges);
        }

        private static Shape BuildSymmetrical(IList<IShape> shapes)
        {
            List<Edge> originalEdges = new List<Edge>();

            foreach (IShape shape in shapes)
            {
                originalEdges.AddRange(shape.GetEdges());
            }

            List<Edge> fragments = new List<Edge>();

            for (int i = 0; i < originalEdges.Count; i++)
            {
                List<Edge> aFragments = new List<Edge> { originalEdges[i] };

                for (int j = 0; j < originalEdges.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    List<Edge> nextFragments = new List<Edge>();

                    foreach (Edge frag in aFragments)
                    {
                        nextFragments.AddRange(frag.Subtract(originalEdges[j]));
                    }

                    aFragments = nextFragments;
                }

                fragments.AddRange(aFragments);
            }

            return new Shape(fragments);
        }
    }
}
