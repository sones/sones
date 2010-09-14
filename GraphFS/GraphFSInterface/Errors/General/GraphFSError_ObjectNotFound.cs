/*
 * GraphFSError_ObjectNotFound
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
    /// A graph object was not found!
    /// </summary>
    public class GraphFSError_ObjectNotFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_ObjectNotFound(myObjectLocation)

        public GraphFSError_ObjectNotFound(String myObjectLocation)
        {
            ObjectLocation = new ObjectLocation(myObjectLocation);
            Message        = String.Format("Graph object '{0}' was not found!", ObjectLocation);
        }

        #endregion

        #region GraphFSError_ObjectNotFound(myObjectLocation)

        public GraphFSError_ObjectNotFound(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("Graph object '{0}' was not found!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
