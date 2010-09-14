/*
 * GraphFSError_CouldNotRemoveDirectoryObject
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// Could not remove the DirectoryObject!
    /// </summary>
    public class GraphFSError_CouldNotRemoveDirectoryObject : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotRemoveDirectoryObject(myObjectLocation)

        public GraphFSError_CouldNotRemoveDirectoryObject(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Could not remove the DirectoryObject at location '{0}'!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
