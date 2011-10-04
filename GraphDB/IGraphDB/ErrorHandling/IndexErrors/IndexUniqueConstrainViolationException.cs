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
    /// An Unique constraint violation on an index of a type has occurred
    /// </summary>
    public sealed class IndexUniqueConstrainViolationException : AGraphDBIndexException
    {
        public String TypeName { get; private set; }
        public String IndexName { get; private set; }

        /// <summary>
        /// Creates a new UniqueConstrainViolationException exception
        /// </summary>
        /// <param name="myTypeName">The name of the given type</param>
        /// <param name="myIndexName">The name of the given index</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IndexUniqueConstrainViolationException(String myTypeName, String myIndexName, Exception innerException = null) : base(innerException)
        {
            TypeName = myTypeName;
            IndexName = myIndexName;
            _msg = String.Format("Unique constraint violation on index {0} of type {1}", IndexName, TypeName);
        }
    }
}

