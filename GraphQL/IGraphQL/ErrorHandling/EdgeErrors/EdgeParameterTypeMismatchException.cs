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
    /// The type of the IncomingEdge parameter does not match
    /// </summary>
    public sealed class EdgeParameterTypeMismatchException : AGraphQLEdgeException
    {
        public String CurrentType { get; private set; }
        public String[] ExpectedTypes { get; private set; }

        /// <summary>
        /// Creates a new EdgeParameterTypeMismatchException exception
        /// </summary>
        /// <param name="currentType"> The current IncomingEdge parameter type</param>
        /// <param name="expectedTypes">A list of expected types</param>
        public EdgeParameterTypeMismatchException(String currentType, params String[] expectedTypes)
        {
            CurrentType = currentType;
            ExpectedTypes = expectedTypes;
            _msg = String.Format("The type [{0}] is not valid. Please use one of [{1}].", CurrentType, ExpectedTypes);
        }
        
    }
}
