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

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The parent vertex type of a vertex type does not exist
    /// </summary>
    public sealed class ParentVertexTypeDoesNotExistException : AGraphDBVertexTypeException
    {
        public String ParentType { get; private set; }
        public String Type { get; private set; }

        /// <summary>
        /// Creates a new ParentVertexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myParentVertexType">The name of the parent type</param>
        /// <param name="myVertexType">The current type</param>
        public ParentVertexTypeDoesNotExistException(String myParentVertexType, String myVertexType)
        {
            ParentType = myParentVertexType;
            Type = myVertexType;
            _msg = String.Format("The parent vertex type {0} of the vertex type {1} does not exist.", ParentType, Type);
        }
 
    }
}
