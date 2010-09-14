/*
 * GraphFSError_CouldNotSetMetadata
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;

using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// Metadata could not be set!
    /// </summary>
    public class GraphFSError_CouldNotSetMetadata : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotSetMetadata(myObjectLocation)

        public GraphFSError_CouldNotSetMetadata(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            ObjectLocation = myObjectLocation;
            ObjectStream   = myObjectStream;
            ObjectEdition  = myObjectEdition;
            Message        = String.Format("Could not set metadata at location '{1}{0}{2}{0}{3}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition);
        }

        #endregion

        #endregion

    }

}
