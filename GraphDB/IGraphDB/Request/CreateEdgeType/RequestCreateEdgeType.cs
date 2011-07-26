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

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new edge type.
    /// </summary>
    public sealed class RequestCreateEdgeType : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the edge that is going to be created.
        /// </summary>
        public EdgeTypePredefinition EdgeTypeDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that creates a new vertex type inside the Graphdb
        /// </summary>
        /// <param name="myVertexTypeDefinition">Describes the vertex that is going to be created</param>
        public RequestCreateEdgeType(EdgeTypePredefinition myEdgeTypeDefinition)
        {
            EdgeTypeDefinition = myEdgeTypeDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion
    }
}
