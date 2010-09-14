/*
 * GraphFSError_CouldNotGetDirectoryListing
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
    /// Could not get the directory listing!
    /// </summary>
    public class GraphFSError_CouldNotGetDirectoryListing : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation    { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotGetDirectoryListing(myObjectLocation)

        public GraphFSError_CouldNotGetDirectoryListing(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Could not get the directory listing of location '{0}'!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
