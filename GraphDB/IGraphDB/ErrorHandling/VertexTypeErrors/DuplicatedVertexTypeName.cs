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
    /// The exception that is thrown, if a bunch of vertex types are added, but the list contains duplicated vertex names.
    /// </summary>
    public sealed class DuplicatedVertexTypeNameException: AGraphDBException
    {
        /// <summary>
        /// The name of the vertex type, that is tried to be added multiple times.
        /// </summary>
        public string DuplicatedName { get; private set; }

        /// <summary>
        /// Creates a new instance of DuplicatedVertexTypeNameException.
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type, that is tried to be added multiple times.</param>
        public DuplicatedVertexTypeNameException(String myVertexTypeName)
        {
            DuplicatedName = myVertexTypeName;
            _msg = string.Format("The vertex type {0} was declared multiple times.", DuplicatedName);
        }

    }
}
