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
    public abstract class AComparativeOperator
    {
        #region data

        /// <summary>
        /// The vertex store that is needed to load the vertices
        /// </summary>
        protected readonly IVertexStore _vertexStore;

        /// <summary>
        /// The current security token
        /// </summary>
        protected readonly SecurityToken _securityToken;

        /// <summary>
        /// The current transaction token
        /// </summary>
        protected readonly TransactionToken _transactionToken;

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
        /// Creates a new comparative operator
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        protected AComparativeOperator(QueryPlanProperty myProperty, ILiteralExpression myConstant, Boolean myIsLongrunning, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexStore myVertexStore)
        {
            _property = myProperty;
            _constant = myConstant;
            _isLongrunning = myIsLongrunning;
            _vertexStore = myVertexStore;
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
        }

        #endregion

        #region abstract definitions

        /// <summary>
        /// Get the vertices that match a certain criteria
        /// </summary>
        /// <param name="myInterestingVertexType">The interesting vertex type</param>
        /// <returns>An enumerable of interesting vertices</returns>
        public abstract IEnumerable<IVertex> GetMatchingVertices(IVertexType myInterestingVertexType);

        #endregion

        #region protected helper

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

            foreach (var aVertex in GetMatchingVertices(myVertexType))
            {
                yield return aVertex;
            }

            #endregion

            #region child types

            foreach (var aChildVertexType in myVertexType.GetDescendantVertexTypes())
            {
                foreach (var aVertex in Execute_protected(aChildVertexType))
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
