namespace sones.GraphDB.Request
{
    /// <summary>
    /// The species of an edge
    /// </summary>
    public enum EdgeSpecies
    {
        /// <summary>
        /// 1-N relation
        /// </summary>
        HyperEdge,

        /// <summary>
        /// 1-1 relation
        /// </summary>
        SingleEdge
    }
}