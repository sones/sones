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
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Defining of incoming IncomingEdge for not referenced attribute is not allowed
    /// </summary>
    public sealed class IncomingEdgeForNotReferenceAttributeTypesException : AGraphDBIncomingEdgeException
    {
        public String VertexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new IncomingEdgeForNotReferenceAttributeTypesException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IncomingEdgeForNotReferenceAttributeTypesException(String myVertexAttributeName, Exception innerException = null) : base(innerException)
        {
            VertexAttributeName = myVertexAttributeName;
            _msg = String.Format("You can not define incoming edges for non reference attribute \"{0}\"!", VertexAttributeName);
        } 
    }
}
