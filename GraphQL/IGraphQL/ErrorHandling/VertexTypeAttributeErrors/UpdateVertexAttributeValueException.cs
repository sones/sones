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

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Could not update a value for a vertex attribute
    /// </summary>
    public sealed class UpdateVertexAttributeValueException : AGraphQLVertexAttributeException
    {
        public String VertexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new UpdateVertexAttributeValueException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        public UpdateVertexAttributeValueException(String myVertexAttributeName)
        {
            VertexAttributeName = myVertexAttributeName;
            _msg = String.Format("Could not update value for vertex attribute \"{0}\".", VertexAttributeName);
        }

    }
}
