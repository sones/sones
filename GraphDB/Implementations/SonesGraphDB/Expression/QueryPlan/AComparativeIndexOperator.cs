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
using sones.GraphDB.Expression.Tree.Literals;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.VertexStore;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.Index;
using sones.GraphDB.ErrorHandling;
using sones.Plugins.Index.Fulltext;
using sones.GraphDB.AdditionalVertices;
using sones.Library.DataStructures;

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

        /// <summary>
        /// The name of the index which is used for resolving the expression
        /// </summary>
        protected readonly String _expressionIndex;

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
        protected AComparativeIndexOperator(QueryPlanProperty myProperty, 
                                            ILiteralExpression myConstant, 
                                            Boolean myIsLongrunning, 
                                            SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken, 
                                            IIndexManager myIndexManager, 
                                            IVertexStore myVertexStore,
                                            String myExpressionIndex = null)
        {
            _property = myProperty;
            _constant = myConstant;
            _isLongrunning = myIsLongrunning;
            _vertexStore = myVertexStore;
            _indexManager = myIndexManager;
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
            _expressionIndex = myExpressionIndex;
        }

        #endregion

        #region abstract definitions

        /// <summary>
        /// Get the best matching index corresponding to the property
        /// </summary>
        /// <param name="myIndexCollection">An enumerable of possible indices</param>
        /// <returns>The chosen one</returns>
        public abstract ISonesIndex GetBestMatchingIdx(
            IEnumerable<ISonesIndex> myIndexCollection);

        #endregion

        #region protected helper

        /// <summary>
        /// Gets the values from an index corresponding to a value
        /// </summary>
        /// <param name="myIndex">The interesting index</param>
        /// <param name="myIComparable">The interesting key</param>
        /// <returns>An enumerable of VertexIDs</returns>
        protected abstract IEnumerable<long> GetValues(ISonesIndex myIndex, 
                                                        IComparable myIComparable);

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
                #region loading the index

                ISonesIndex idx = null;

                //an index name is specified in an expression, try to get the index
                //
                //NOTE: if the specified index couldn't be found, null will be returnd
                //      to use another index, the call of GetBestMatchingIndex could be added here
                if (_expressionIndex != null)
                    idx = _indexManager.GetIndex(_expressionIndex, 
                                                    _securityToken, 
                                                    _transactionToken);
                //no index name is specified, get the best matching index
                else
                    idx = GetBestMatchingIdx(_indexManager.GetIndices(myVertexType, 
                                                                        _property.Property, 
                                                                        _securityToken, 
                                                                        _transactionToken));

                //no index could be found
                if (idx == null)
                    throw new IndexDoesNotExistException(
                                _expressionIndex == null ? "no index found" : _expressionIndex, 
                                "default");
                
                #endregion

                #region get the index results

                //index is a ISonesFulltextIndex, 
                //so we make a query and get a ISonesFulltextResult
                if (idx is ISonesFulltextIndex)
                {
                    var value = (idx as ISonesFulltextIndex).Query(_constant.Value.ToString());

                    if (value != null)
                    {
                        //read out the ISonesFulltextResultEntries
                        //create TemporalVertices which store
                        vertices.AddRange(value.Entries
                                               .OrderByDescending(entry => entry.Score)
                                               .Select(entry => CreateTemporalVertex(
                                                                GetVertex(entry.VertexID, myVertexType),
                                                                entry)));
                    }
                }
                else
                {
                    //read out the values
                    var values = GetValues(idx, _constant.Value);

                    if (values != null)
                    {
                        foreach (var aVertexID in values)
                        {
                            vertices.Add(GetVertex(aVertexID, myVertexType));
                        }
                    }
                }

                #endregion
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

        #region helper

        private IVertex GetVertex(long myVertexID, IVertexType myVertexType)
        {
            return _vertexStore.GetVertex(_securityToken,
                                            _transactionToken,
                                            myVertexID,
                                            myVertexType.ID,
                                            VertexEditionFilter,
                                            VertexRevisionFilter);
        }

        /// <summary>
        /// Creates a TemporalVertex with the <paramref name="myVertex"/> 
        /// and the FulltextResultEntry of the index.
        /// </summary>
        /// <param name="myVertex">
        /// The vertex which is used to create the temporal vertex.
        /// </param>
        /// <param name="myEntry">
        /// The FulltextResultEntry of the index query.
        /// </param>
        /// <returns>
        /// A new TemporalVertex object.
        /// </returns>
        private IVertex CreateTemporalVertex(IVertex myVertex, ISonesFulltextResultEntry myEntry)
        {
            var result =  new TemporalVertex(myVertex);
            result.AddTemporalProperty("ISonesFulltextResultEntry", myEntry);

            return result;
        }

        #endregion
    }
}
