/*
 * GraphFSError_CouldNotDeserialize
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
    public class GraphFSError_CouldNotDeserialize : GraphFSError
    {

        #region Properties

        public Type Type { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotDeserialize(myType)

        public GraphFSError_CouldNotDeserialize(Type myType)
        {
            Type    = myType;
            Message = String.Format("Could not deserialize type '{0}'!", Type);
        }

        #endregion

        #endregion

    }

}
