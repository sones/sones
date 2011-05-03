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
        /// A set of typed single edges.
        /// </summary>
        MultiEdge,
        
        /// <summary>
        /// A set of untyped single edges
        /// </summary>
        HyperEdge
    }
}
