namespace sones.GraphDB.Result
{
    /// <summary>
    /// The interface for all single IncomingEdge views
    /// </summary>
    public interface ISingleEdgeView : IEdgeView
    {
        /// <summary>
        /// Returns the target vertex view
        /// </summary>
        /// <returns>The target vertex view</returns>
        IVertexView GetTargetVertex();
    }
}
