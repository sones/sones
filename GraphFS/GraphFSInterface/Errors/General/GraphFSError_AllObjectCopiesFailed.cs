/*
 * GraphFSError_AllObjectCopiesFailed
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
    /// All object copies failed!
    /// </summary>
    public class GraphFSError_AllObjectCopiesFailed : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }
        public ObjectRevisionID     RevisionID     { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_AllObjectCopiesFailed(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        public GraphFSError_AllObjectCopiesFailed(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            ObjectEdition   = myObjectEdition;
            RevisionID      = myRevisionID;
            Message         = String.Format("All object copies at location '{1}{0}{2}{0}{3}{0}{4}' failed!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition, RevisionID);
        }

        #endregion

        #endregion

    }

}
