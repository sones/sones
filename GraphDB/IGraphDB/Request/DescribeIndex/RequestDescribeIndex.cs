using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request.GetIndex
{
    public sealed class RequestDescribeIndex : IRequest
    {
        #region data

        /// <summary>
        /// The interesting type on which the index is created
        /// </summary>
        public readonly String TypeName;

        /// <summary>
        /// The interesting index name
        /// </summary>
        public readonly String IndexName;

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
        /// Creates a new request gets a vertex type from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeName">The interesting vertex type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestDescribeIndex(String myTypeName, String myIndexName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            TypeName = myTypeName;
            IndexName = myIndexName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
