/*
 * GraphFSError_NoFileSystemMounted
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// No file system had been mounted! Please mount a file system first!
    /// </summary>
    public class GraphFSError_NoFileSystemMounted : GraphFSError
    {

        #region Constructor

        #region GraphFSError_NoFileSystemMounted(myType)

        public GraphFSError_NoFileSystemMounted()
        {
            Message = String.Format("No file system had been mounted! Please mount a file system first!");
        }

        #endregion

        #endregion

    }

}
