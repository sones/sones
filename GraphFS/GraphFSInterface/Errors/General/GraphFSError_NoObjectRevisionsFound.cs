/*
 * GraphFSError_NoObjectRevisionsFound
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
    /// No object revisions found!
    /// </summary>
    public class GraphFSError_NoObjectRevisionsFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_NoObjectRevisionsFound(myObjectLocation, myObjectStream, myObjectEdition)

        public GraphFSError_NoObjectRevisionsFound(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            ObjectEdition   = myObjectEdition;
            Message         = String.Format("No object revisions found at location '{1}{0}{2}{0}{3}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition);
        }

        #endregion

        #endregion

    }

}
