/*
 * GraphFSError_DirectoryObjectNotFound
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
    /// A directory object was not found!
    /// </summary>
    public class GraphFSError_DirectoryObjectNotFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_DirectoryObjectNotFound(myObjectLocation)

        public GraphFSError_DirectoryObjectNotFound(String myObjectLocation)
        {
            ObjectLocation = new ObjectLocation(myObjectLocation);
            Message        = String.Format("Directory object '{0}' was not found!", ObjectLocation);
        }

        #endregion

        #region GraphFSError_DirectoryObjectNotFound(myObjectLocation)

        public GraphFSError_DirectoryObjectNotFound(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Directory object '{0}' was not found!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
