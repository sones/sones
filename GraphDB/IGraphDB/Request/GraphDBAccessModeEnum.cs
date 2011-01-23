using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The different GraphDB access modes
    /// </summary>
    public enum GraphDBAccessModeEnum
    {
        /// <summary>
        /// vertex or edge type changes like alter, create
        /// </summary>
        TypeChange,
        
        /// <summary>
        /// clear
        /// </summary>
        WriteOnly,
        
        /// <summary>
        /// graph traversal, get vertex
        /// </summary>
        ReadOnly,
        
        /// <summary>
        /// insert, update ...
        /// </summary>
        ReadWrite
    }
}
