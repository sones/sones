using System;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain vertex already exist
    /// </summary>
    public sealed class VertexAlreadyExistException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the type of the vertex
        /// </summary>
        public readonly Int64 TypeID;

        /// <summary>
        /// The id of the desired vertex
        /// </summary>
        public readonly Int64 VertexID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new VertexAlreadyExistException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id of the vertex</param>
        /// <param name="myVertexID">The id of the vertex</param>
        public VertexAlreadyExistException(Int64 myTypeID, Int64 myVertexID)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
        }

        #endregion
        
    }
}