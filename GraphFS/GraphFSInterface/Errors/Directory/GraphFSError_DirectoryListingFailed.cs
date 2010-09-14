/*
 * GraphFSError_DirectoryListingFailed
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
    /// Directory listing failed!
    /// </summary>
    public class GraphFSError_DirectoryListingFailed : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_DirectoryListingFailed(myObjectLocation)

        public GraphFSError_DirectoryListingFailed(String myObjectLocation)
        {
            ObjectLocation = new ObjectLocation(myObjectLocation);
            Message        = String.Format("Directory listing of location '{0}' failed!", ObjectLocation);
        }

        #endregion

        #region GraphFSError_DirectoryListingFailed(myObjectLocation)

        public GraphFSError_DirectoryListingFailed(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Directory listing of location '{0}' failed!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
