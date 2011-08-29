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
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    public sealed class RequestGetAllEdgeTypes : IRequest
    {
        #region data

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request gets a edge type from the Graphdb
        /// </summary>
        /// <param name="myEdgeTypeName">The interesting edge type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetAllEdgeTypes(String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}
