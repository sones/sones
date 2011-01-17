using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Context;
using sones.Library.Internal.Security;

namespace sones.GraphDB
{
    /// <summary>
    /// The class that creates a connection to a graphdb implementation
    /// </summary>
    public sealed class GraphDBConnection
    {
        #region data

        /// <summary>
        /// The connection to the graph
        /// </summary>
        public readonly IGraphDBSession GraphSession = null;

        #endregion

        #region Constructor

        public GraphDBConnection(IGraphDB myIGraphDB, Credentials rootCreds)
        {
            GraphSession = myIGraphDB.GetSession(rootCreds);
        }

        #endregion
    }
}
