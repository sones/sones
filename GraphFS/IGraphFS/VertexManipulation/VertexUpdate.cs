using System;
using sones.PropertyHyperGraph;

namespace sones.GraphFS
{
    /// <summary>
    /// The vertex update definition
    /// 
    /// This class contains the updates for a vertex
    /// </summary>
    public sealed class VertexUpdate
    {
        #region data

        /// <summary>
        /// The vertex id of the vertex that is going to be updated
        /// </summary>
        public readonly VertexID myVertexID;

        #endregion
        
        #region constructor

        public VertexUpdate(VertexID myVertexID)
        {

        }

        #endregion
    }
}
