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
using System.Text.RegularExpressions;
using sones.GraphDB.Expression.Tree.Literals;
using sones.GraphDB.Manager.Index;
using sones.Library.Commons.Security;
using sones.Library.Commons.VertexStore;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.Index;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A Like operation using indices
    /// </summary>
    public sealed class QueryPlanLikeWithIndex : AComparativeIndexOperator, IQueryPlan
    {
        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a like operation using indices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myExpressionIndex">The index name which is used for resolving the expression</param>
        public QueryPlanLikeWithIndex(SecurityToken mySecurityToken, 
                                        Int64 myTransactionToken, 
                                        QueryPlanProperty myProperty, 
                                        ILiteralExpression myConstant, 
                                        IVertexStore myVertexStore, 
                                        Boolean myIsLongrunning, 
                                        IIndexManager myIndexManager,
                                        String myExpressionIndex = null)
            : base(myProperty, 
                    myConstant, 
                    myIsLongrunning, 
                    mySecurityToken, 
                    myTransactionToken, 
                    myIndexManager, 
                    myVertexStore,
                    myExpressionIndex)
        { }

        #endregion

        #region IQueryPlan Members
        
        public IEnumerable<IVertex> Execute()
        {
            return Execute_protected(_property.VertexType);
        }

        #endregion

        #region overrides

        public override ISonesIndex GetBestMatchingIdx(IEnumerable<ISonesIndex> myIndexCollection)
        {
            return myIndexCollection.First();
        }

        protected override IEnumerable<long> GetValues(ISonesIndex myIndex, IComparable myIComparable)
        {
            var regexpression = new Regex((String)myIComparable);
            List<long> ids = new List<long>();

            foreach (var key in myIndex.Keys())
            {
                if (regexpression.IsMatch((String)key))
                {
                    ids.AddRange(myIndex[key]);
                }
            }

            return ids;
        }

        #endregion
    }
}