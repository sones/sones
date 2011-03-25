namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an edge definition on a vertex type definition.
    /// </summary>
    public interface IEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The type of the edge. Never <c>NULL</c>.
        /// </summary>
        IEdgeType EdgeType { get; }

        /// <summary>
        /// The source vertex type. Never <c>NULL</c>.
        /// </summary>
        /// <remarks>
        /// On this vertex type this edge is defined as outgoing edge.
        /// </remarks>
        IVertexType SourceVertexType { get; }

        /// <summary>
        /// The target vertex type. Never <c>NULL</c>.
        /// </summary>
        /// <remarks>
        /// On this vertex type this edge is defined as incoming edge.
        /// </remarks>
        IVertexType TargetVertexType { get; }
    }
}
