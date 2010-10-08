/*
 * GraphFSError_NoINodePositionsForReading
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
    /// A graph object could not be read as no INode positions had been found!
    /// </summary>
    public class GraphFSError_NoINodePositionsForReading : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_NoINodePositionsForReading()

        public GraphFSError_NoINodePositionsForReading()
        {
            ObjectLocation = null;
            Message        = String.Format("No INode positions found for reading!");
        }

        #endregion

        #region GraphFSError_NoINodePositionsForReading(myObjectLocation)

        public GraphFSError_NoINodePositionsForReading(ObjectLocation myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
            Message        = String.Format("No INode positions found for reading the graph object at location '{0}'!", ObjectLocation.ToString());
        }

        #endregion

        #endregion

    }

}
