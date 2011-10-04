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
    /// The user defined vertex type should not be used with LIST attributes
    /// </summary>
    public sealed class ListAttributeNotAllowedException : AGraphDBVertexTypeException
    {
        public String VertexTypeName { get; private set; }
        
        /// <summary>
        /// Creates a new ListAttributeNotAllowedException exception
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public ListAttributeNotAllowedException(String myVertexTypeName, Exception innerException = null) : base(innerException)
        {
            VertexTypeName = myVertexTypeName;
            _msg = String.Format("The user defined vertex type \\{0}\\ should not be used with LIST<> attributes, please use SET<> instead.", VertexTypeName);
        }

    }
}
