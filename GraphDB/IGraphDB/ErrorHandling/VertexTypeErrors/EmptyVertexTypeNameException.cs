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
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type with an empty name should was used.
    /// </summary>
    public sealed class EmptyVertexTypeNameException: AGraphDBVertexTypeException
    {
        /// <summary>
        /// Creates an instance of EmptyVertexTypeNameException.
        /// </summary>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public EmptyVertexTypeNameException(Exception innerException = null) : base(innerException)
        {
            _msg = "This operation needs a valid vertex type name.";
        }
    }
}
