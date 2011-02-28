using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;

namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// The information / loaction definition of a vertex
    /// </summary>
    public struct VertexInformation
    {
        #region data

        /// <summary>
        /// The type id of this vertex
        /// </summary>
        public readonly UInt64 VertexTypeID;
        
        /// <summary>
        /// The id of this vertex
        /// </summary>
        public readonly UInt64 VertexID;
        
        /// <summary>
        /// The vertex revision id
        /// </summary>
        public readonly VertexRevisionID VertexRevisionID;
        
        /// <summary>
        /// The vertex edition name
        /// </summary>
        public readonly String VertexEditionName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex information/location
        /// </summary>
        /// <param name="myVertexTypeID">The type id of this vertex</param>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexRevisionID">The vertex revision id</param>
        /// <param name="myVertexEditionName">The vertex edition name</param>
        public VertexInformation(
            UInt64 myVertexTypeID, 
            UInt64 myVertexID,
            VertexRevisionID myVertexRevisionID,
            String myVertexEditionName)
        {
            VertexTypeID = myVertexTypeID;
            VertexID = myVertexID;
            VertexRevisionID = myVertexRevisionID;
            VertexEditionName = myVertexEditionName;
        }

        #endregion

    }
}
