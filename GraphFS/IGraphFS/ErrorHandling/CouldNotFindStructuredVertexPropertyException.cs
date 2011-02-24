using sones.ErrorHandling;
using System;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain structured vertex property does not exist
    /// </summary>
    public sealed class CouldNotFindStructuredVertexPropertyException : AGraphFSException
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

        /// <summary>
        /// The id of the desired property
        /// </summary>
        public readonly UInt64 PropertyID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotFindStructuredVertexPropertyException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myPropertyID">The desired property of the vertex</param>
        public CouldNotFindStructuredVertexPropertyException(UInt64 myTypeID, UInt64 myVertexID, UInt64 myPropertyID)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
            PropertyID = myPropertyID;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotFindStructuredVertexProperty; }
        }
    }
}