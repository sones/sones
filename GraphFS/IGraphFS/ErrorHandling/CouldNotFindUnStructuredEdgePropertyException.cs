using System;
using sones.Library.ErrorHandling;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain unstructured edge property does not exist
    /// </summary>
    public sealed class CouldNotFindUnStructuredEdgePropertyException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The name of the desired property
        /// </summary>
        public readonly String PropertyName;

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
        /// <param name="myPropertyName">The desired property name of the edge</param>
        public CouldNotFindUnStructuredEdgePropertyException(Int64 myTypeID, String myPropertyName)
        {
            TypeID = myTypeID;
            PropertyName = myPropertyName;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotFindUnStructuredEdgeProperty; }
        }
    }
}