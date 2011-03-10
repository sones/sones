using System;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertex statistics
    /// </summary>
    public interface IVertexStatistics : IGraphElementStatistics
    {
        #region Degree

        /// <summary>
        /// For a vertex, the number of incoming edges is called the indegree 
        /// </summary>
        UInt64 InDegree { get; }

        /// <summary>
        /// For a vertex, the number outgoing edges is called the outdegree 
        /// </summary>
        UInt64 OutDegree { get; }

        /// <summary>
        /// For a vertex the number of incoming plus the number of outgoing edges is called the degree
        /// </summary>
        UInt64 Degree { get; }

        #endregion
    }
}