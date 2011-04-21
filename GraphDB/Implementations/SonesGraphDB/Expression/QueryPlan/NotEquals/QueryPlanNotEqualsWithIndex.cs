using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Index;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Plugins.Index.Interfaces;
using System.Linq;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A not equals operation using indices
    /// </summary>
    public sealed class QueryPlanNotEqualsWithIndex : AComparativeIndexOperator, IQueryPlan
    {
        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a not equals operation using indices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanNotEqualsWithIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, ILiteralExpression myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning, IIndexManager myIndexManager)
            :base(myProperty, myConstant, myIsLongrunning, mySecurityToken, myTransactionToken, myIndexManager, myVertexStore)
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

        public override IIndex<IComparable, long> GetBestMatchingIdx(IEnumerable<IIndex<IComparable, long>> myIndexCollection)
        {
            return myIndexCollection.First();
        }
        
        public override IEnumerable<long> GetSingleIndexValues(ISingleValueIndex<IComparable, long> mySingleValueIndex, IComparable myIComparable)
        {
            return mySingleValueIndex.Where(kv => kv.Key.CompareTo(myIComparable) != 0).Select(kv => kv.Value);
        }

        public override IEnumerable<long> GetMultipleIndexValues(IMultipleValueIndex<IComparable, long> myMultipleValueIndex, IComparable myIComparable)
        {
            foreach (var aVertexIDSet in myMultipleValueIndex.Where(kv => kv.Key.CompareTo(myIComparable) != 0).Select(kv => kv.Value))
            {
                foreach (var aVertexID in aVertexIDSet)
                {
                    yield return aVertexID;
                }
            }

            yield break;
        }

        #endregion
    }
}