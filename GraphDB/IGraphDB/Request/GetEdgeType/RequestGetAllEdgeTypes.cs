using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request.GetEdgeType
{
    public sealed class RequestGetAllEdgeTypes : IRequest
    {
        #region data

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request gets a edge type from the Graphdb
        /// </summary>
        /// <param name="myEdgeTypeName">The interesting edge type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetAllEdgeTypes(String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}
