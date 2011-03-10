using System;
using sones.Library.ErrorHandling;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain structured edge property does not exist
    /// </summary>
    public sealed class CouldNotFindStructuredEdgePropertyException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the desired property
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The id of the type of the edge
        /// </summary>
        public readonly Int64 TypeID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotFindStructuredEdgePropertyException exception
        /// </summary>
        /// <param name="myTypeID">The edge type id</param>
        /// <param name="myPropertyID">The desired property of the edge</param>
        public CouldNotFindStructuredEdgePropertyException(Int64 myTypeID, Int64 myPropertyID)
        {
            TypeID = myTypeID;
            PropertyID = myPropertyID;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotFindStructuredEdgeProperty; }
        }
    }
}