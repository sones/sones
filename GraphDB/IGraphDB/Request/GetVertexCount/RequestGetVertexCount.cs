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
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertex count request
    /// </summary>
    public sealed class RequestGetVertexCount : IRequest
    {
        #region data

        /// <summary>
        /// The interesting vertex type id
        /// </summary>
        public readonly Int64 VertexTypeID;

        /// <summary>
        /// The interesting vertex type name
        /// </summary>
        public readonly String VertexTypeName;

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
        /// Creates a new request that gets a vertex count from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeName">The interesting vertex type name</param>
        /// <param name="myEdition">The interesting edition</param>
        /// <param name="myTimeSpanDefinition">The interesting timespan</param>
        public RequestGetVertexCount(String myVertexTypeName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = myVertexTypeName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        /// <summary>
        /// Creates a new request that gets a vertex count from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeId">The interesting vertex type id</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetVertexCount(Int64 myVertexTypeId, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeID = myVertexTypeId;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
            VertexTypeName = null;
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
