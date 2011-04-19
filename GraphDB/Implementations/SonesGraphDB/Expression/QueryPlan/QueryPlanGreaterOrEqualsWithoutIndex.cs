using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An GreaterOrEquals operation without any index
    /// </summary>
    public sealed class QueryPlanGreaterOrEqualsWithoutIndex : IQueryPlan
    {
        #region data

        /// <summary>
        /// The interesting property
        /// </summary>
        private readonly QueryPlanProperty _property;

        /// <summary>
        /// The constant value
        /// </summary>
        private readonly QueryPlanConstant _constant;

        /// <summary>
        /// The vertex store that is needed to load the vertices
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        private readonly Boolean _isLongrunning;

        /// <summary>
        /// The current security token
        /// </summary>
        private readonly SecurityToken _securityToken;

        /// <summary>
        /// The current transaction token
        /// </summary>
        private readonly TransactionToken _transactionToken;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an equals operation without any index
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanGreaterOrEqualsWithoutIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, QueryPlanConstant myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning)
        {
            _property = myProperty;
            _constant = myConstant;
            _vertexStore = myVertexStore;
            _isLongrunning = myIsLongrunning;
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            return Execute_private(_property.VertexType);
        }

        #endregion

        #region private helper

        /// <summary>
        /// Checks the revision of a vertex
        /// </summary>
        /// <param name="myToBeCheckedID">The revision that needs to be checked</param>
        /// <returns>True or false</returns>
        private bool VertexRevisionFilter(Int64 myToBeCheckedID)
        {
            return _property.Timespan.IsWithinTimeStamp(myToBeCheckedID);
        }

        /// <summary>
        /// Get the matching vertices correspondig to this plan
        /// </summary>
        /// <param name="myVertexTypeID">The interesting vertex type id</param>
        /// <returns>An enumerable of vertices</returns>
        private IEnumerable<IVertex> GetMatchingVertices(long myVertexTypeID)
        {
            foreach (var aVertex in _vertexStore.GetVerticesByTypeID(_securityToken, _transactionToken, myVertexTypeID, _property.Edition, VertexRevisionFilter))
            {
                if (aVertex.HasProperty(_property.Property.AttributeID))
                {
                    if (_property.Property.ExtractValue(aVertex).CompareTo(_constant.Constant) >= 0)
                    {
                        yield return aVertex;
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Recursive walk through the vertex type hierarchy
        /// </summary>
        /// <param name="myVertexType">The starting vertex type</param>
        /// <returns>An enumerable of matching vertices</returns>
        private IEnumerable<IVertex> Execute_private(IVertexType myVertexType)
        {
            #region current type

            foreach (var aVertex in GetMatchingVertices(myVertexType.ID))
            {
                yield return aVertex;
            }

            #endregion

            #region child types

            foreach (var aChildVertexType in myVertexType.GetChildVertexTypes())
            {
                foreach (var aVertex in Execute_private(aChildVertexType))
                {
                    yield return aVertex;
                }
            }

            #endregion

            yield break;
        }

        #endregion
    }
}