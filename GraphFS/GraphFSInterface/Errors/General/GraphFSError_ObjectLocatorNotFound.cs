/*
 * GraphFSError_ObjectLocatorNotFound
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
    /// ObjectLocator not found!
    /// </summary>
    public class GraphFSError_ObjectLocatorNotFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_ObjectLocatorNotFound(myObjectLocation)

        public GraphFSError_ObjectLocatorNotFound(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
            Message         = String.Format("ObjectLocator of location '{0}' not found!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
