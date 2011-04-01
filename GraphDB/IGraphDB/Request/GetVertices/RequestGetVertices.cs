using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertices request
    /// </summary>
    public sealed class RequestGetVertices : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertices that should be requests from the graphdb
        /// </summary>
        public readonly GetVerticesDefinition GetVerticesDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myGetVerticesDefinition">The definition of the vertices that should be requests from the graphdb</param>
        public RequestGetVertices(GetVerticesDefinition myGetVerticesDefinition)
        {
            GetVerticesDefinition = myGetVerticesDefinition;
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
