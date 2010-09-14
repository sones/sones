/*
 * GraphFSError_CouldNotGetObjectLocator
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
    /// Could not get ObjectLocator!
    /// </summary>
    public class GraphFSError_CouldNotGetObjectLocator : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation    { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotGetObjectLocator(myObjectLocation)

        public GraphFSError_CouldNotGetObjectLocator(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Could not get ObjectLocator of location '{0}'!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
