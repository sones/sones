/*
 * GraphFSError_MakeFileSystemFailed
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// A graph object was not found!
    /// </summary>
    public class GraphFSError_MakeFileSystemFailed : GraphFSError
    {

        #region Constructor

        #region GraphFSError_MakeFileSystemFailed()

        public GraphFSError_MakeFileSystemFailed()
        {
            Message = String.Format("MakeFileSystem failed!");
        }

        #endregion

        #region GraphFSError_MakeFileSystemFailed(myMessage)

        public GraphFSError_MakeFileSystemFailed(String myMessage)
        {
            Message = myMessage;
        }

        #endregion

        #region GraphFSError_MakeFileSystemFailed(myFormatedMessage, myObjects)

        public GraphFSError_MakeFileSystemFailed(String myFormatedMessage, params Object[] myObjects)
        {
            Message = String.Format(myFormatedMessage, myObjects);
        }

        #endregion

        #endregion

    }

}
