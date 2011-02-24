using sones.ErrorHandling;
using System;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain unstructured vertex property does not exist
    /// </summary>
    public sealed class CouldNotFindUnStructuredVertexPropertyException : AGraphFSException
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
        public readonly String PropertyName;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotFindUnStructuredVertexPropertyException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myPropertyID">The desired property of the vertex</param>
        public CouldNotFindUnStructuredVertexPropertyException(UInt64 myTypeID, UInt64 myVertexID, String myPropertyName)
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