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
        /// The id of the desired property
        /// </summary>
        public readonly Int64 PropertyID;

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
        /// Creates a new CouldNotFindStructuredVertexPropertyException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myPropertyID">The desired property of the vertex</param>
        public CouldNotFindStructuredVertexPropertyException(Int64 myTypeID, Int64 myVertexID, Int64 myPropertyID)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
            PropertyID = myPropertyID;
        }

        #endregion
                
    }
}