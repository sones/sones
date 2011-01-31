using System;
using System.Collections.Generic;

namespace sones.GraphInfrastructure.Element
{
    /// <summary>
    /// The interface for graph element ids
    /// </summary>
    public interface IGraphElementID
    {
        /// <summary>
        /// The ID of the graph element type
        /// </summary>
        String TypeID { get; }
    }
}
