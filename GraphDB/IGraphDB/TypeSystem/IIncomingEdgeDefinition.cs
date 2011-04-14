namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents a definition of an incoming IncomingEdge.
    /// An incoming IncomingEdge is definied by the outgoing IncomingEdge that shares the same IncomingEdge.
    /// </summary>
    public interface IIncomingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The related outgoing IncomingEdge. Never <c>NULL</c>.
        /// </summary>
        IOutgoingEdgeDefinition RelatedEdgeDefinition { get; }
    }
}
