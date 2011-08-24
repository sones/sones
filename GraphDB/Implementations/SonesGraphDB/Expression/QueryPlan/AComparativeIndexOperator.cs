/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Interfaces;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression.QueryPlan
{
    public abstract class AComparativeIndexOperator
    {
        #region data

        /// <summary>
        /// The vertex store that is needed to load the vertices
        /// </summary>
        protected readonly IVertexStore _vertexStore;

        /// <summary>
        /// The index manager is needed to get the property related indices
        /// </summary>
        protected readonly IIndexManager _indexManager;

        /// <summary>
        /// The current security token
        /// </summary>
        protected readonly SecurityToken _securityToken;

        /// <summary>
        /// The current transaction token
        /// </summary>
        protected readonly Int64 _transactionToken;

        /// <summary>
        /// The interesting property
        /// </summary>
        protected readonly QueryPlanProperty _property;

        /// <summary>
        /// The constant value
        /// </summary>
        protected readonly ILiteralExpression _constant;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        protected readonly Boolean _isLongrunning;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new comparative index operator
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myIndexManager">The index manager is needed to get the property related indices</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        protected AComparativeIndexOperator(QueryPlanProperty myProperty, ILiteralExpression myConstant, Boolean myIsLongrunning, SecurityToken mySecurityToken, Int64 myTransactionToken, IIndexManager myIndexManager, IVertexStore myVertexStore)
        {
            _property = myProperty;
            _constant = myConstant;
            _isLongrunning = myIsLongrunning;
            _vertexStore = myVertexStore;
            _indexManager = myIndexManager;
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
        }

        #endregion

        #region abstract definitions

        /// <summary>
        /// Extract values from a single value index
        /// </summary>
        /// <param name="mySingleValueIndex">The interesting index</param>
        /// <param name="myIComparable">The interesting key</param>
        /// <returns>An enumerable of vertexIDs</returns>
        public abstract IEnumerable<long> GetSingleIndexValues(ISingleValueIndex<IComparable, Int64> mySingleValueIndex,
                                             IComparable myIComparable);

        /// <summary>
        /// Extract values from a multiple value index
        /// </summary>
        /// <param name="mySingleValueIndex">The interesting index</param>
        /// <param name="myIComparable">The interesting key</param>
        /// <returns>An enumerable of vertexIDs</returns>
        public abstract IEnumerable<long> GetMultipleIndexValues(IMultipleValueIndex<IComparable, Int64> mySingleValueIndex,
                                             IComparable myIComparable);

        /// <summary>
        /// Get the best matching index corresponding to the property
        /// </summary>
        /// <param name="myIndexCollection">An enumerable of possible indices</param>
        /// <returns>The chosen one</returns>
        public abstract IIndex<IComparable, long> GetBestMatchingIdx(
            IEnumerable<IIndex<IComparable, long>> myIndexCollection);

        #endregion

        #region protected helper

        /// <summary>
        /// Gets the values from an index corresponding to a value
        /// </summary>
        /// <param name="myIndex">The interesting index</param>
        /// <param name="myIComparable">The interesting key</param>
        /// <returns>An enumerable of VertexIDs</returns>
        protected IEnumerable<long> GetValues(IIndex<IComparable, long> myIndex, IComparable myIComparable)
        {
            if (myIndex is ISingleValueIndex<IComparable, Int64>)
            {
                foreach (var aVertexID in GetSingleIndexValues((ISingleValueIndex<IComparable, Int64>) myIndex, myIComparable))
                {
                    yield return aVertexID; 
                }
            }
            else
            {
                if (myIndex is IMultipleValueIndex<IComparable, Int64>)
                {
                    foreach (var aVertexID in GetMultipleIndexValues((IMultipleValueIndex<IComparable, Int64>)myIndex, myIComparable))
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
        protected bool VertexRevisionFilter(Int64 myToBeCheckedID)
        {
            return _property.Timespan.IsWithinTimeStamp(myToBeCheckedID);
        }

        /// <summary>
        /// Checks the edition of a vertex
        /// </summary>
        /// <param name="myToBeCheckedEdition">The edition that needs to be checked</param>
        /// <returns>True or false</returns>
        protected bool VertexEditionFilter(String myToBeCheckedEdition)
        {
            return _property.Edition == myToBeCheckedEdition;
        }

        /// <summary>
        /// Executes the query plan recursivly
        /// </summary>
        /// <param name="myVertexType">The starting vertex type</param>
        /// <returns>An enumeration of vertices</returns>
        protected IEnumerable<IVertex> Execute_protected(IVertexType myVertexType)
        {
            #region current type

            List<IVertex> vertices = new List<IVertex>();

            if (!myVertexType.IsAbstract)
            {
                var idx = GetBestMatchingIdx(_indexManager.GetIndices(myVertexType, _property.Property, _securityToken, _transactionToken));

                foreach (var aVertexID in GetValues(idx, _constant.Value))
                {
                    vertices.Add(_vertexStore.GetVertex(_securityToken, _transactionToken, aVertexID, myVertexType.ID, VertexEditionFilter, VertexRevisionFilter));
                }    
            }

            #endregion

            #region child types

            foreach (var aChildVertexType in myVertexType.ChildrenVertexTypes)
            {
                vertices.AddRange(Execute_protected(aChildVertexType));
            }

            #endregion

            return vertices;
        }

        #endregion

    }
}
