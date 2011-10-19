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
    /// The index type does not exists
    /// </summary>
    public sealed class IndexTypeDoesNotExistException : AGraphDBIndexException
    {
        public String IndexName { get; private set; }
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new IndexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexTypeName"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IndexTypeDoesNotExistException(String myType, String myIndexName, Exception innerException = null) : base(innerException)
        {
            IndexName = myIndexName;
            TypeName = myType;

            _msg = String.Format("The index named \"{0}\" does not exist on type \"{1}\"!", IndexName, TypeName);
        }
      
    }
}
