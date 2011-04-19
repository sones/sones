namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The multiplicity of properties
    /// </summary>
    public enum PropertyMultiplicity: byte
    {
        /// <summary>
        /// The property is a simple one like an integer or a string
        /// </summary>
        Single,
        
        /// <summary>
        /// The property is organized as a list which allows to have duplicate entries
        /// </summary>
        List,
        
        /// <summary>
        /// The property is organized as a set
        /// </summary>
        Set
    }
}
