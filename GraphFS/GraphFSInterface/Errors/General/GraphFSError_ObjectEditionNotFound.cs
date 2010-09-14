/*
 * GraphFSError_ObjectEditionNotFound
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
    /// Object edition not found!
    /// </summary>
    public class GraphFSError_ObjectEditionNotFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_ObjectEditionNotFound(myObjectLocation, myObjectStream, myObjectEdition)

        public GraphFSError_ObjectEditionNotFound(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            ObjectEdition   = myObjectEdition;
            Message         = String.Format("Object edition '{0}' at location '{1}{2}{3}' not found!", ObjectEdition, ObjectLocation, FSPathConstants.PathDelimiter, ObjectStream);
        }

        #endregion

        #endregion

    }

}
