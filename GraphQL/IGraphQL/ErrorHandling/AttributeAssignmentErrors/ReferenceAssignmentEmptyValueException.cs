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
    /// A single reference attribute does not contain any value
    /// </summary>
    public sealed class ReferenceAssignmentEmptyValueException : AGraphQLAttributeAssignmentException
    {
        public String AttributeName { get; private set; }

        /// <summary>
        /// Creates a new ReferenceAssignmentEmptyValueException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public ReferenceAssignmentEmptyValueException(String myAttributeName, Exception innerException = null) : base(innerException)
        {
            AttributeName = myAttributeName;
            _msg = String.Format("The single reference attribute {0} does not contain any value.", AttributeName);
        }
        
    }
}
