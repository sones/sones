namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all single-target edges
    /// </summary>
    public interface ISingleEdge : IEdge
    {
        /// <summary>
        /// Returns the target vertex
        /// </summary>
        /// <returns>The target vertex</returns>
        IVertex GetTargetVertex();
    }
}