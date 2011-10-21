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
    /// Too many elements for a type of an IncomingEdge 
    /// </summary>
    public sealed class TooManyElementsForEdgeException : AGraphQLEdgeException
    {
        public UInt64 CurrentElements { get; private set; }
        public String EdgeTypeName { get; private set; }

        /// <summary>
        /// Creates a new TooManyElementsForEdgeException exception
        /// </summary>
        /// <param name="edgeTypeName">The name of the IncomingEdge type</param>
        /// <param name="currentElements">The current count of elements</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public TooManyElementsForEdgeException(String edgeTypeName, UInt64 currentElements, Exception innerException = null) : base(innerException)
        {
            CurrentElements = currentElements;
            EdgeTypeName = edgeTypeName;
            _msg = String.Format("The edge [{0}] does not take {1} elements.", EdgeTypeName, CurrentElements);
        }

    }
}
