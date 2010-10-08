/*
 * GraphFSError_CouldNotGetObjectStreams
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
    /// Could not get object streams!
    /// </summary>
    public class GraphFSError_CouldNotGetObjectStreams : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation    { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotGetObjectStreams(myObjectLocation)

        public GraphFSError_CouldNotGetObjectStreams(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
            Message         = String.Format("Could not get object streams at location '{0}'!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
