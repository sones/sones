namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for edge properties
    /// </summary>
    public interface IEdgeProperties
    {
        #region Statistics

        /// <summary>
        /// The statistics of an edge
        /// </summary>
        IEdgeStatistics Statistics { get; }

        #endregion
    }
}