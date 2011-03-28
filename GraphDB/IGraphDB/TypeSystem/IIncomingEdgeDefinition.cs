namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents a definition of an incoming edge.
    /// An incoming edge is definied by the outgoing edge that shares the same edge.
    /// </summary>
    public interface IIncomingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The related outgoing edge. Never <c>NULL</c>.
        /// </summary>
        IOutgoingEdgeDefinition RelatedEdgeDefinition { get; }
    }
}
