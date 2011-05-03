namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an outgoing edge definition on a vertex type definition.
    /// </summary>
    public interface IOutgoingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The type of the edge. Never <c>NULL</c>.
        /// </summary>
        IEdgeType EdgeType { get; }

        /// <summary>
        /// The type of the inner edges of an multi edge. Might be <c>NULL</c>.
        /// </summary>
        IEdgeType InnerEdgeType { get; }

        /// <summary>
        /// The source vertex type. Never <c>NULL</c>.
        /// </summary>
        IVertexType SourceVertexType { get; }

        /// <summary>
        /// The target vertex type. Never <c>NULL</c>.
        /// </summary>
        IVertexType TargetVertexType { get; }

        /// <summary>
        /// The multiplicity of this outgoing edge definition.
        /// </summary>
        EdgeMultiplicity Multiplicity { get; }
    }
}
