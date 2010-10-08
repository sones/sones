/*
 * GraphFSError_CouldNotWipeFileSystem
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Errors
{

    public class GraphFSError_CouldNotWipeFileSystem : GraphFSError
    {

        #region Properties

        public String    StorageLocation { get; private set; }
        public Exception Exception       { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotWipeFileSystem(myStorageLocation, myException)

        public GraphFSError_CouldNotWipeFileSystem(String myStorageLocation, Exception myException)
        {
            StorageLocation = myStorageLocation;
            Exception       = myException;
            Message         = String.Format("Could not wipe file system at storage location '{0}', caused by '{1}'!", myStorageLocation, Exception.ToString());
        }

        #endregion

        #endregion

    }

}
