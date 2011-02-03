using System;
using System.Collections.Generic;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for graph element statistics
    /// </summary>
    public interface IGraphElementStatistics
    {
        /// <summary>
        /// The number of visits of this graph element
        /// </summary>
        UInt64 Visits { get; }
    }
}
