/*
 * GraphFSError_ObjectStreamNotFound
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
    /// Object stream not found!
    /// </summary>
    public class GraphFSError_ObjectStreamNotFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream)

        public GraphFSError_ObjectStreamNotFound(ObjectLocation myObjectLocation, String myObjectStream)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            Message         = String.Format("Object stream '{0}' at location '{1}' not found!", ObjectStream, ObjectLocation);
        }

        #endregion

        #endregion

    }

}
