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
    /// An invalid group level when adding a group element to a selection
    /// </summary>
    public sealed class InvalidGroupByLevelException : AGraphQLSelectException
    {
        public String IDChainDefinitionEdgesCount { get; private set; }
        public String IDChainDefinitionContentString { get; private set; }

        /// <summary>
        /// Creates a new InvalidGroupByLevelException exception
        /// </summary>
        /// <param name="myIDChainDefinitionEdgesCount">The count of edges of the IDChainDefinition</param>
        /// <param name="myIDChainDefinitionContentString"></param>
        public InvalidGroupByLevelException(String myIDChainDefinitionEdgesCount, String myIDChainDefinitionContentString)
        {
            IDChainDefinitionEdgesCount = myIDChainDefinitionEdgesCount;
            IDChainDefinitionContentString = myIDChainDefinitionContentString;
            _msg = String.Format("The level ({0}) greater than 1 is not allowed: '{1}'", IDChainDefinitionEdgesCount, IDChainDefinitionContentString);

        }

    }
}
