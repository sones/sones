using System;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain vertex type does not exist
    /// </summary>
    public sealed class VertexTypeDoesNotExistException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the type of the vertex
        /// </summary>
        public readonly Int64 TypeID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new VertexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id of the vertex</param>
        public VertexTypeDoesNotExistException(Int64 myTypeID)
        {
            TypeID = myTypeID;
            _msg = String.Format("The vertex type with id {0} does not exist.", myTypeID);
        }

        #endregion

    }
}