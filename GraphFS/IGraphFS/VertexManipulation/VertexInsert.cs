using System;
using sones.PropertyHyperGraph;

namespace sones.GraphFS
{
    /// <summary>
    /// The vertex insert definition
    /// 
    /// This class contains the properties/edges/etc of a new vertex
    /// </summary>
    public sealed class VertexInsert
    {
        #region data

        /// <summary>
        /// The vertex id of the vertex that is going to be inserted
        /// </summary>
        public readonly VertexID myVertexID;

        #endregion

        #region constructor

        public VertexInsert(VertexID myVertexID)
        {

        }

        #endregion
    }
}
