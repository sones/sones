using System;

namespace sones.GraphInfrastructure.Element
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
