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

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type should derive from a sealed vertex type.
    /// </summary>
    public sealed class SealedBaseVertexTypeException: AGraphDBVertexTypeException
    {
        /// <summary>
        /// The vertex type that causes the error.
        /// </summary>
        public string VertexTypeName { get; private set; }

        /// <summary>
        /// The sealed parent vertex type.
        /// </summary>
        public string ParentVertexTypeName { get; private set; }

        /// <summary>
        /// Creates an instance of SealedBaseVertexTypeException.
        /// </summary>
        /// <param name="myVertexTypeName">
        /// The vertex type that causes the error.
        /// </param>
        /// <param name="myParentVertexTypeName">
        /// The sealed parent vertex type.
        /// </param>
        public SealedBaseVertexTypeException(string myVertexTypeName, string myParentVertexTypeName)
        {
            this.VertexTypeName = myVertexTypeName;
            this.ParentVertexTypeName = myParentVertexTypeName;
            _msg = string.Format("Vertex type {0} can not derive from sealed vertex type {1}.", myVertexTypeName, myParentVertexTypeName);
        }
    }
}
