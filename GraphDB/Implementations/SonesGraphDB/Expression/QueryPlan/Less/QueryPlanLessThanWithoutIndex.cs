using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An less than operation without any index
    /// </summary>
    public sealed class QueryPlanLessThanWithoutIndex : AComparativeOperator, IQueryPlan
    {
        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a less than operation without any index
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanLessThanWithoutIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, ILiteralExpression myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning)
            : base(myProperty, myConstant, myIsLongrunning, mySecurityToken, myTransactionToken, myVertexStore)            
        {
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            return Execute_protected(_property.VertexType);
        }

        #endregion

        #region overrides

        public override IEnumerable<IVertex> GetMatchingVertices(IVertexType myInterestingVertexType)
        {
            foreach (var aVertex in _vertexStore.GetVerticesByTypeID(_securityToken, _transactionToken, myInterestingVertexType.ID, _property.Edition, VertexRevisionFilter))
            {
                if (aVertex.HasProperty(_property.Property.AttributeID))
                {
                    if (_property.Property.ExtractValue(aVertex).CompareTo(_constant.Value) < 0)
                    {
                        yield return aVertex;
                    }
                }
            }

            yield break;
        }

        #endregion
    }
}