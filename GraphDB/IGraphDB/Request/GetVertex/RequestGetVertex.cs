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
    /// The get vertex request
    /// </summary>
    public sealed class RequestGetVertex : IRequest
    {
        #region data

        /// <summary>
        /// The vertex type name of the requested vertex
        /// </summary>
        public readonly String VertexTypeName;

        /// <summary>
        /// The vertex type id of the requested vertex
        /// </summary>
        public readonly Int64 VertexTypeID;

        /// <summary>
        /// The id of the requested vertex
        /// </summary>
        public readonly Int64 VertexID;

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
        /// Creates a new request gets a vertex from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetVertex(String myVertexTypeName, Int64 myVertexID, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = myVertexTypeName;
            VertexID = myVertexID;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        /// <summary>
        /// Creates a new request gets a vertex from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type id of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetVertex(Int64 myVertexTypeID, Int64 myVertexID, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = null;
            VertexID = myVertexID;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
            VertexTypeID = myVertexTypeID;
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
