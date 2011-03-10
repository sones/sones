using System;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all informations concerning the graph partition
    /// </summary>
    public interface IGraphPartitionInformation
    {
        /// <summary>
        /// The partition id
        /// </summary>
        Int64 PartitionID { get; }
    }
}