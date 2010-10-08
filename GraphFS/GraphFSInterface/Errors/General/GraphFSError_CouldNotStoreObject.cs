/*
 * GraphFSError_CouldNotStoreObject
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
    /// Metadata could not be set!
    /// </summary>
    public class GraphFSError_CouldNotStoreObject : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotStoreObject(myObjectLocation)

        public GraphFSError_CouldNotStoreObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            ObjectLocation = myObjectLocation;
            ObjectStream   = myObjectStream;
            ObjectEdition  = myObjectEdition;
            Message        = String.Format("Could not store object at location '{1}{0}{2}{0}{3}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition);
        }

        #endregion

        #endregion

    }

}
