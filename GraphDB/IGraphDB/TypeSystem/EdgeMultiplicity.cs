namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The multiplicity of edges.
    /// </summary>
    public enum EdgeMultiplicity: byte
    {
        /// <summary>
        /// The edge is a single edge.
        /// </summary>
        SingleEdge,
        
        /// <summary>
        /// The edge is organized as a set.
        /// </summary>
        HyperEdge
    }
}
