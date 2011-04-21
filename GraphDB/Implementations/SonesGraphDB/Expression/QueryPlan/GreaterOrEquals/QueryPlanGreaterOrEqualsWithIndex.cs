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
    /// An GreaterOrEquals operation using indices
    /// </summary>
    public sealed class QueryPlanGreaterOrEqualsWithIndex : AComparativeIndexOperator, IQueryPlan
    {
        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an GreaterOrEquals operation using indices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanGreaterOrEqualsWithIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, ILiteralExpression myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning, IIndexManager myIndexManager)
            : base(myProperty, myConstant, myIsLongrunning, mySecurityToken, myTransactionToken, myIndexManager, myVertexStore)        
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
            if (mySingleValueIndex is IRangeIndex<IComparable, long>)
            {
                //use the range funtionality

                foreach (var aVertexID in ((ISingleValueRangeIndex<IComparable, long>)mySingleValueIndex).GreaterThan(myIComparable))
                {
                    yield return aVertexID;
                }
            }
            else
            {
                //stupid, but works

                foreach (var aVertexID in mySingleValueIndex.Where(kv => kv.Key.CompareTo(myIComparable) >= 0).Select(kv => kv.Value))
                {
                    yield return aVertexID;
                }

            }

            yield break;
        }

        public override IEnumerable<long> GetMultipleIndexValues(IMultipleValueIndex<IComparable, long> myMultipleValueIndex, IComparable myIComparable)
        {
            if (myMultipleValueIndex is IRangeIndex<IComparable, long>)
            {
                //use the range funtionality

                foreach (var aVertexIDSet in ((IMultipleValueRangeIndex<IComparable, long>)myMultipleValueIndex).GreaterThan(myIComparable))
                {
                    foreach (var aVertexID in aVertexIDSet)
                    {
                        yield return aVertexID;
                    }
                }
            }
            else
            {
                //stupid, but works

                foreach (var aVertexIDSet in myMultipleValueIndex.Where(kv => kv.Key.CompareTo(myIComparable) >= 0).Select(kv => kv.Value))
                {
                    foreach (var aVertexID in aVertexIDSet)
                    {
                        yield return aVertexID;
                    }
                }
            }

            yield break;
        }

        #endregion
    }
}