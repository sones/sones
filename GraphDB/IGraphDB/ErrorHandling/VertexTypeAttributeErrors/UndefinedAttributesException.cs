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
    /// Undefined attributes can not inserted nor updated without setting SETUNDEFBEHAVE 
    /// </summary>
    public sealed class UndefinedAttributesException : AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates a new UndefinedAttributesException exception
        /// </summary>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public UndefinedAttributesException(Exception innerException = null) : base(innerException)
        {
            _msg = "Undefined attributes can not inserted nor updated. Use the setting SETUNDEFBEHAVE to change this behaviour.";
        }

    }
}
