using System;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// The information / loaction definition of a vertex
    /// </summary>
    public sealed class VertexInformation
    {
        #region data

        /// <summary>
        /// The vertex edition name
        /// </summary>
        public readonly String VertexEditionName;

        /// <summary>
        /// The id of this vertex
        /// </summary>
        public readonly Int64 VertexID;

        /// <summary>
        /// The vertex revision id
        /// </summary>
        public readonly VertexRevisionID VertexRevisionID;

        /// <summary>
        /// The type id of this vertex
        /// </summary>
        public readonly Int64 VertexTypeID;

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
            Int64 myVertexTypeID,
            Int64 myVertexID,
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