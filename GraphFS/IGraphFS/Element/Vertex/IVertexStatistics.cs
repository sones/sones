using System;
using System.Collections.Generic;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for vertex statistics
    /// </summary>
    public interface IVertexStatistics
    {
        /// <summary>
        /// For a vertex, the number of incoming edges is called the indegree 
        /// </summary>
        /// <returns>An unsigned value</returns>
        UInt64 GetInDegree();

        /// <summary>
        /// For a vertex, the number outgoing edges is called the outdegree 
        /// </summary>
        /// <returns>An unsigned value</returns>
        UInt64 GetOutDegree();

        /// <summary>
        /// For a vertex the number of incoming plus the number of outgoing edges is called the degree
        /// </summary>
        /// <returns>An unsigned value</returns>
        UInt64 GetDegree();
    }
}
