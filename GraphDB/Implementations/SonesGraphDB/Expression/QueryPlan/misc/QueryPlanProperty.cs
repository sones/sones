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

using sones.GraphDB.TypeSystem;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.GraphDB.ErrorHandling.QueryPlan;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A vertexType/property combination
    /// </summary>
    public sealed class QueryPlanProperty : IQueryPlan
    {
        #region data

        /// <summary>
        /// The vertex type
        /// </summary>
        public readonly IVertexType VertexType;

        /// <summary>
        /// The interesting property
        /// </summary>
        public readonly IPropertyDefinition Property;

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new query plan property
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type</param>
        /// <param name="myProperty">The interesting property</param>
        public QueryPlanProperty(IVertexType myVertexType, IPropertyDefinition myProperty, String myInterestingEdition, TimeSpanDefinition myInterestingTimeSpan)
        {
            VertexType = myVertexType;
            Property = myProperty;
            Edition = myInterestingEdition;
            Timespan = myInterestingTimeSpan;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            throw new InvalidQueryPlanExecutionException("It is not possible to execute a query plan property.");
        }

        #endregion
    }
}