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
    /// A in range operation without any index
    /// </summary>
    public sealed class QueryPlanInRangeWithoutIndex : AComparativeOperator, IQueryPlan
    {
        #region data

        /// <summary>
        /// The constant range
        /// </summary>
        private readonly RangeLiteralExpression _constantRange;

        #endregion
        

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a in range operation without any index
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanInRangeWithoutIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, RangeLiteralExpression myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning)
            : base(myProperty, null, myIsLongrunning, mySecurityToken, myTransactionToken, myVertexStore)
        {
            _constantRange = myConstant;
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
            if (_constantRange.IncludeBorders)
            {
                foreach (var aVertex in _vertexStore.GetVerticesByTypeID(_securityToken, _transactionToken, myInterestingVertexType.ID, _property.Edition, VertexRevisionFilter))
                {
                    var value = _property.Property.ExtractValue(aVertex);

                    if (value != null &&
                            (value.CompareTo(_constantRange.Lower) >= 0) &&
                            (value.CompareTo(_constantRange.Upper) <= 0))
                    {
                        yield return aVertex;
                    }
                }
            }
            else
            {
                foreach (var aVertex in _vertexStore.GetVerticesByTypeID(_securityToken, _transactionToken, myInterestingVertexType.ID, _property.Edition, VertexRevisionFilter))
                {
                    var value = _property.Property.ExtractValue(aVertex);

                    if (value != null &&
                            (value.CompareTo(_constantRange.Lower) > 0) &&
                            (value.CompareTo(_constantRange.Upper) < 0))
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