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
    /// A to group attribute is not selected
    /// </summary>
    public sealed class GroupedAttributeIsNotSelectedException : AGraphQLSelectException
    {
        public String TypeAttribute { get; private set; }

        /// <summary>
        /// Creates a new GroupedAttributeIsNotSelectedException exception
        /// </summary>
        /// <param name="myTypeAttribute">The name of the type attribute</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public GroupedAttributeIsNotSelectedException(String myTypeAttribute, Exception innerException = null) : base(innerException)
        {
            TypeAttribute = myTypeAttribute;
            _msg = String.Format("The attribute '{0}' is not selected and can not be grouped.", TypeAttribute);
        }
        
    }
}
