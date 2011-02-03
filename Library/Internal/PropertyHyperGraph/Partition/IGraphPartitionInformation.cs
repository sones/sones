using System;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all informations concerning the graph partition
    /// </summary>
    public interface IGraphPartitionInformation
    {
        /// <summary>
        /// The partition id
        /// </summary>
        UInt64 PartitionID { get; }
    }
}
