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
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An OR operation sequentially executed
    /// </summary>
    public sealed class QueryPlanORSequentiell : IQueryPlan
    {
        #region data

        /// <summary>
        /// The left query plan
        /// </summary>
        private readonly IQueryPlan _left;
        
        /// <summary>
        /// The right query plan
        /// </summary>
        private readonly IQueryPlan _right;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        private readonly Boolean _isLongrunning;

        #endregion

        #region constructor

        public QueryPlanORSequentiell(IQueryPlan myLeft, IQueryPlan myRight, Boolean myIsLongrunning)
        {
            _left = myLeft;
            _right = myRight;
            _isLongrunning = myIsLongrunning;
        }

        #endregion

        #region IQueryPlan Members
        
        public IEnumerable<IVertex> Execute()
        {
            return _left.Execute().Union(_right.Execute());
        }

        #endregion
    }
}