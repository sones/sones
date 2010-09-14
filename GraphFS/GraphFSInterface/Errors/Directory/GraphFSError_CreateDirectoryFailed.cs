/*
 * GraphFSError_CreateDirectoryFailed
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
    /// Creating a directory failed!
    /// </summary>
    public class GraphFSError_CreateDirectoryFailed : GraphFSError
    {
        private   DataStructures.ObjectLocation myObjectLocation;


        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CreateDirectoryFailed(myObjectLocation)

        public GraphFSError_CreateDirectoryFailed(ObjectLocation myObjectLocation, String myMessage = null)
        {
            ObjectLocation = myObjectLocation;

            if (myMessage != null)
            {
                Message = String.Format("Creating a directory at location '{0}' failed because: {1}", ObjectLocation, myMessage);
            }
            else
            {
                Message = String.Format("Creating a directory at location '{0}' failed!", ObjectLocation);
            }
        }

        #endregion

        #endregion

    }

}
