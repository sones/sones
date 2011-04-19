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

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An equals operation using indices
    /// </summary>
    public sealed class QueryPlanEqualsWithIndex : IQueryPlan
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
        /// The index manager is needed to get the property related indices
        /// </summary>
        private readonly IIndexManager _indexManager;

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
        /// Creates a new queryplan that processes an equals operation using indices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanEqualsWithIndex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, QueryPlanProperty myProperty, QueryPlanConstant myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning, IIndexManager myIndexManager)
        {
            _property = myProperty;
            _constant = myConstant;
            _vertexStore = myVertexStore;
            _isLongrunning = myIsLongrunning;
            _indexManager = myIndexManager;
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
        /// Gets the values from an index corresponding to a value
        /// </summary>
        /// <param name="myIndex">The interesting index</param>
        /// <param name="myIComparable">The interesting key</param>
        /// <returns></returns>
        private IEnumerable<long> GetValues(IIndex<IComparable, long> myIndex, IComparable myIComparable)
        {
            if (myIndex is ISingleValueIndex<IComparable, Int64>)
            {
                yield return ((ISingleValueIndex<IComparable, Int64>)myIndex)[myIComparable];
            }
            else
            {
                if (myIndex is IMultipleValueIndex<IComparable, Int64>)
                {
                    foreach (var aVertexID in ((IMultipleValueIndex<IComparable, Int64>)myIndex)[myIComparable])
                    {
                        yield return aVertexID;
                    }
                }
                else
                {
                    //there might be a little more interfaces... sth versioned
                }
            }

            yield break;
        }

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
        /// Checks the edition of a vertex
        /// </summary>
        /// <param name="myToBeCheckedEdition">The edition that needs to be checked</param>
        /// <returns>True or false</returns>
        private bool VertexEditionFilter(String myToBeCheckedEdition)
        {
            return _property.Edition == myToBeCheckedEdition;
        }

        /// <summary>
        /// Executes the query plan recursivly
        /// </summary>
        /// <param name="myVertexType">The starting vertex type</param>
        /// <returns>An enumeration of vertices</returns>
        private IEnumerable<IVertex> Execute_private(IVertexType myVertexType)
        {
            #region current type

            var idx = GetBestMatchingIdx(_indexManager.GetIndices(myVertexType, _property.Property, _securityToken, _transactionToken));

            if (idx.ContainsKey(_constant.Constant))
            {
                foreach (var aVertex in GetValues(idx, _constant.Constant)
                    .Select(aId => _vertexStore.GetVertex(_securityToken, _transactionToken, aId, myVertexType.ID, VertexEditionFilter, VertexRevisionFilter)))
                {
                    yield return aVertex;
                }
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

        /// <summary>
        /// Get the best matching index out of a index collection
        /// </summary>
        /// <param name="myIndexCollection">The interesting collection of indices</param>
        /// <returns></returns>
        private IIndex<IComparable, long> GetBestMatchingIdx(IEnumerable<IIndex<IComparable, long>> myIndexCollection)
        {
            return myIndexCollection.First();
        }

        #endregion
    }
}