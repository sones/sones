/*
 * GraphFSError_CouldNotOverwriteRevision
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
    public class GraphFSError_CouldNotOverwriteRevision : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation    { get; private set; }
        public String         ObjectStream      { get; private set; }
        public String         ObjectEdition     { get; private set; }
        public ObjectRevisionID     ObjectRevisionID  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotOverwritteRevision(myObjectLocation, myObjectStream)

        public GraphFSError_CouldNotOverwriteRevision(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID)
        {
            ObjectLocation      = myObjectLocation;
            ObjectStream        = myObjectStream;
            ObjectEdition       = myObjectEdition;
            ObjectRevisionID    = myObjectRevisionID;
            Message             = String.Format("Could not overwrite object revision '{1}{0}{2}{0}{3}{0}{4}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition, ObjectRevisionID);
        }

        #endregion

        #endregion

    }

}
