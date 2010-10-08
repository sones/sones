/*
 * GraphFSError_NoObjectEditionsFound
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
    /// No object editions found!
    /// </summary>
    public class GraphFSError_NoObjectEditionsFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_NoObjectEditionsFound(myObjectLocation, myObjectStream)

        public GraphFSError_NoObjectEditionsFound(ObjectLocation myObjectLocation, String myObjectStream)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            Message         = String.Format("No object editions found at location '{1}{0}{2}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream);
        }

        #endregion

        #endregion

    }

}
