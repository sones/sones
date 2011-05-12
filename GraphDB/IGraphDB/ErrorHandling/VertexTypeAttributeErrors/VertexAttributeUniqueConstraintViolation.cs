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
    /// The unique constraint of an attribute of an vertex type has been violated
   /// </summary>
    public sealed class VertexAttributeUniqueConstraintViolation : AGraphDBVertexAttributeException
    {
        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }

       /// <summary>
        /// Creates a new VertexAttributeUniqueConstraintViolation exception
       /// </summary>
       /// <param name="myVertexTypeName">The name of given the vertex type</param>
       /// <param name="myVertexAttributeName">The name of the given vertex attribute</param>
        public VertexAttributeUniqueConstraintViolation(String myVertexTypeName, String myVertexAttributeName)
       {
           VertexAttributeName = myVertexAttributeName;
           VertexTypeName = myVertexTypeName;
           _msg = String.Format("The unique constraint of the attribute {0} of the vertex type {1} has been violated.", VertexAttributeName, VertexTypeName);
       }

    }
}
