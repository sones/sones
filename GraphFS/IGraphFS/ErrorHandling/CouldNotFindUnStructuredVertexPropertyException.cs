using System;
using sones.ErrorHandling;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain unstructured vertex property does not exist
    /// </summary>
    public sealed class CouldNotFindUnStructuredVertexPropertyException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the desired property
        /// </summary>
        public readonly String PropertyName;

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
        /// Creates a new CouldNotFindUnStructuredVertexPropertyException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myPropertyName">The desired property of the vertex</param>
        public CouldNotFindUnStructuredVertexPropertyException(Int64 myTypeID, Int64 myVertexID, String myPropertyName)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
            PropertyName = myPropertyName;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotFindUnStructuredVertexProperty; }
        }
    }
}