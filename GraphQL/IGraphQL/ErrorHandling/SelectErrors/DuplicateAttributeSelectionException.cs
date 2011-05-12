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
    /// A duplicate attribute selection is not allowed
    /// </summary>
    public sealed class DuplicateAttributeSelectionException : AGraphQLSelectException
    {
        public String SelectionAlias { get; private set; }

        /// <summary>
        /// Creates a new DuplicateAttributeSelectionException exception
        /// </summary>
        /// <param name="mySelectionAlias">The alias to use</param>
        public DuplicateAttributeSelectionException(String mySelectionAlias)
        {
            SelectionAlias = mySelectionAlias;
            _msg = String.Format("You cannot select \"{0}\" more than one time. Try to use an alias.", SelectionAlias);
        }
        
    }
}
