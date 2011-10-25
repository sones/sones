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
using sones.GraphDB.Manager.Index;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.VertexStore;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.Index;
using sones.Plugins.Index.Range;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A in range operation using indices
    /// </summary>
    public sealed class QueryPlanInRangeWithIndex : IQueryPlan
    {
        #region data

        /// <summary>
        /// The vertex store that is needed to load the vertices
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// The index manager is needed to get the property related indices
        /// </summary>
        private readonly IIndexManager _indexManager = null;

        /// <summary>
        /// The current security token
        /// </summary>
        private readonly SecurityToken _securityToken;

        /// <summary>
        /// The current transaction token
        /// </summary>
        private readonly Int64 _transactionToken;

        /// <summary>
        /// The interesting property
        /// </summary>
        private readonly QueryPlanProperty _property;

        /// <summary>
        /// The constant value
        /// </summary>
        private readonly RangeLiteralExpression _constant;        

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        private readonly Boolean _isLongrunning;

        /// <summary>
        /// The name of the index which is used for resolving the expression
        /// </summary>
        protected readonly String _expressionIndex;
        
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a in range operation using indices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myExpressionIndex">The index name which is used for resolving the expression</param>
        public QueryPlanInRangeWithIndex(SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken, 
                                            QueryPlanProperty myProperty, 
                                            RangeLiteralExpression myConstant, 
                                            IVertexStore myVertexStore, 
                                            Boolean myIsLongrunning, 
                                            IIndexManager myIndexManager,
                                            String myExpressionIndex = null)
        {
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
            _property = myProperty;
            _constant = myConstant;
            _vertexStore = myVertexStore;
            _isLongrunning = myIsLongrunning;
            _expressionIndex = myExpressionIndex;
        }

        #endregion

        #region IQueryPlan Members
        
        public IEnumerable<IVertex> Execute()
        {
            return Execute_private(_property.VertexType);
        }

        #endregion

        #region private methods

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

            ISonesIndex idx = null;

            if (_expressionIndex != null)
                idx = _indexManager.GetIndex(_expressionIndex, _securityToken, _transactionToken);
            else
                idx = GetBestMatchingIdx(_indexManager.GetIndices(_property.Property, _securityToken, _transactionToken));

            foreach (var aVertex in GetValues(idx, _constant)
                                    .Select(aId => _vertexStore.GetVertex(
                                                                    _securityToken, 
                                                                    _transactionToken, 
                                                                    aId, 
                                                                    myVertexType.ID, 
                                                                    VertexEditionFilter, 
                                                                    VertexRevisionFilter)))
            {
                yield return aVertex;
            }

            #endregion

            #region child types

            foreach (var aChildVertexType in myVertexType.GetDescendantVertexTypes())
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
        /// Extract the values corresponding to the range
        /// </summary>
        /// <param name="myIndex">The interesting index</param>
        /// <param name="myConstant">The inderesting range</param>
        /// <returns>An enumerable of vertexIDs</returns>
        private IEnumerable<long> GetValues(ISonesIndex myIndex, RangeLiteralExpression myConstant)
        {
            if (myIndex is ISonesRangeIndex)
            {
                //use the range funtionality
                foreach (var aVertexID in ((ISonesRangeIndex)myIndex)
                    .Between(myConstant.Lower, myConstant.Upper, myConstant.IncludeBorders, myConstant.IncludeBorders))
                {
                    yield return aVertexID;
                }
            }
            else
            {
                //stupid, but works

                if (myConstant.IncludeBorders)
                {
                    foreach (var aVertexIDSet in myIndex.Keys()
                        .Where(key =>
                            (key.CompareTo(myConstant.Lower) >= 0) &&
                            (key.CompareTo(myConstant.Upper) <= 0))
                                .Select(key => myIndex[key]))
                    {
                        foreach (var aVertexID in aVertexIDSet)
                        {
                            yield return aVertexID;
                        }
                    }
                }
                else
                {
                    foreach (var aVertexIDSet in myIndex.Keys()
                        .Where(key =>
                            (key.CompareTo(myConstant.Lower) > 0) &&
                            (key.CompareTo(myConstant.Upper) < 0))
                                .Select(key => myIndex[key]))
                    {
                        foreach (var aVertexID in aVertexIDSet)
                        {
                            yield return aVertexID;
                        }
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Get the best matching index corresponding to the property
        /// </summary>
        /// <param name="myIndexCollection">An enumerable of possible indices</param>
        /// <returns>The chosen one</returns>
        private ISonesIndex GetBestMatchingIdx(IEnumerable<ISonesIndex> myIndexCollection)
        {
            return myIndexCollection.First();
        }

        #endregion
    }
}