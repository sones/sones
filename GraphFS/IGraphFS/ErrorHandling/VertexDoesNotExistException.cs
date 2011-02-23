using sones.ErrorHandling;
using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A certain vertex does not exist
    /// </summary>
    public sealed class VertexDoesNotExistException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the type of the vertex
        /// </summary>
        public readonly UInt64 TypeID;

        /// <summary>
        /// The id of the desired vertex
        /// </summary>
        public readonly UInt64 VertexID;

        #endregion

        #region constructor

        public VertexDoesNotExistException(UInt64 myTypeID, UInt64 myVertexID)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.VertexDoesNotExist; }
        }
    }
}