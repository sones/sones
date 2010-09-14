/*
 * GraphFSError_INodeCouldNotBeLoaded
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
    /// The requested INode could not be loadedd!
    /// </summary>
    public class GraphFSError_INodeCouldNotBeLoaded : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_INodeCouldNotBeLoaded(myObjectLocation, myObjectStream)

        public GraphFSError_INodeCouldNotBeLoaded(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
            Message         = String.Format("The INode(s) of object location '{0}' could not been loaded!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
