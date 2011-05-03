using sones.GraphDB.Expression;
using System;
using System.Collections;
using System.Collections.Generic;
namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertices request
    /// </summary>
    public sealed class RequestGetVertices : IRequest
    {
        #region data

        /// <summary>
        /// The interesting vertex type 
        /// </summary>
        public readonly String VertexTypeName;

        /// <summary>
        /// The interesting vertex type 
        /// </summary>
        public readonly Int64 VertexTypeID;

        /// <summary>
        /// The interesting vertex ids
        /// </summary>
        public readonly IEnumerable<Int64> VertexIDs;

        /// <summary>
        /// The expression which should be evaluated
        /// </summary>
        public readonly IExpression Expression;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        public readonly Boolean IsLongrunning;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that gets vertices from the Graphdb
        /// ...uses expressions to select vertices
        /// </summary>
        /// <param name="myExpression">The expression which should be evaluated</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public RequestGetVertices(IExpression myExpression, Boolean myIsLongrunning = false)
        {
            Expression = myExpression;
            IsLongrunning = myIsLongrunning;
        }

        public RequestGetVertices(Int64 myVertexTypeID, IEnumerable<Int64> myVertexIDs, Boolean myIsLongrunning = false)
        {
            VertexTypeID = myVertexTypeID;
            VertexIDs = myVertexIDs;

            Expression = null;
            IsLongrunning = myIsLongrunning;
        }

            /// <summary>
        /// Creates a new request that gets vertices from the Graphdb
        /// ...uses direct access to vertices via their ids and a vertex type id
        /// </summary>
        /// <param name="myVertexTypeID">The interesting vertex type id</param>
        /// <param name="myVertexIDs">The interersting vertex ids</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public RequestGetVertices(String myVertexType, IEnumerable<Int64> myVertexIDs, Boolean myIsLongrunning = false)
        {
            VertexTypeName = myVertexType;
            VertexIDs = myVertexIDs;

            Expression = null;
            IsLongrunning = myIsLongrunning;
        }

        /// <summary>
        /// Creates a new request that gets vertices from the Graphdb
        /// ...uses direct access to vertices via a vertex type id, so every vertex to a given type id is loaded
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type.</param>
        public RequestGetVertices(String myVertexType)
        {
            VertexTypeName = myVertexType;
            
            VertexIDs = null;
            Expression = null;
            IsLongrunning = true;
        }

        /// <summary>
        /// Creates a new request that gets vertices from the Graphdb
        /// ...uses direct access to vertices via a vertex type id, so every vertex to a given type id is loaded
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type id.</param>
        public RequestGetVertices(Int64 myVertexTypeID)
        {
            VertexTypeID = myVertexTypeID;

            VertexIDs = null;
            Expression = null;
            IsLongrunning = true;
        }

        #endregion

        #region IRequest Members

        GraphDBAccessMode IRequest.AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}
