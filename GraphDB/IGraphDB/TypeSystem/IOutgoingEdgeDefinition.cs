namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an IncomingEdge definition on a vertex type definition.
    /// </summary>
    public interface IOutgoingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The type of the IncomingEdge. Never <c>NULL</c>.
        /// </summary>
        IEdgeType EdgeType { get; }

        /// <summary>
        /// The source vertex type. Never <c>NULL</c>.
        /// </summary>
        /// <remarks>
        /// On this vertex type this IncomingEdge is defined as outgoing IncomingEdge.
        /// </remarks>
        IVertexType SourceVertexType { get; }

        /// <summary>
        /// The target vertex type. Never <c>NULL</c>.
        /// </summary>
        /// <remarks>
        /// On this vertex type this IncomingEdge is defined as incoming IncomingEdge.
        /// </remarks>
        IVertexType TargetVertexType { get; }
    }
}
