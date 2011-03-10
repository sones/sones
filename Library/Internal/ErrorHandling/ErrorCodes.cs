using System;

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
        public static UInt16 UnknownError = 100;

        #endregion

        #region IGraphFS (prefix: 2)

        /// <summary>
        /// An unknown GraphFS error
        /// </summary>
        public static UInt16 UnknownFSError = 200;

        /// <summary>
        /// A certain vertex does not exist
        /// </summary>
        public static UInt16 VertexDoesNotExist = 201;

        /// <summary>
        /// A certain structured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindStructuredVertexProperty = 202;

        /// <summary>
        /// A certain unstructured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredVertexProperty = 203;

        /// <summary>
        /// A certain structured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindStructuredEdgeProperty = 204;

        /// <summary>
        /// A certain unstructured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredEdgeProperty = 205;

        #endregion

        #region IGraphDB (prefix: 3)

        /// <summary>
        /// An unknown GraphDB error
        /// </summary>
        public static UInt16 UnknownDBError = 300;

        #endregion

        #region Library (prefix: 4)

        /// <summary>
        /// An assembly file could not loaded
        /// </summary>
        public static UInt16 CouldNotLoadAssembly = 401;

        /// <summary>
        /// A plugin version is incompatible
        /// </summary>
        public static UInt16 IncompatiblePluginVersion = 402;


        #endregion
    }
}