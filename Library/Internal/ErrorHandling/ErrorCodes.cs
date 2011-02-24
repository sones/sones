using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.ErrorHandling
{
    /// <summary>
    /// Contains static ErrorCodes
    /// </summary>
    public static class ErrorCodes
    {
        #region General (prefix: 1)
        
        /// <summary>
        /// A totally unknown error
        /// </summary>
        public static UInt16 UnknownError                           = 100;

        #endregion

        #region IGraphFS (prefix: 2)

        /// <summary>
        /// An unknown GraphFS error
        /// </summary>
        public static UInt16 UnknownFSError                         = 200;
        
        /// <summary>
        /// A certain vertex does not exist
        /// </summary>
        public static UInt16 VertexDoesNotExist                     = 201;
        
        /// <summary>
        /// A certain structured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindStructuredVertexProperty   = 202;

        /// <summary>
        /// A certain unstructured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredVertexProperty = 203;

        #endregion

        #region IGraphDB (prefix: 3)

        /// <summary>
        /// An unknown GraphDB error
        /// </summary>
        public static UInt16 UnknownDBError                         = 300;

        #endregion

    }
}
