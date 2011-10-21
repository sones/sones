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
    /// The number of parameters of an IncomingEdge does not match
    /// </summary>
    public sealed class EdgeParameterCountMismatchException : AGraphQLEdgeException
    {
        public String Edge { get; private set; }
        public UInt32 CurrentNumOfParams { get; private set; }
        public UInt32 ExpectedNumOfParams { get; private set; }

        /// <summary>
        /// Creates a new EdgeParameterCountMismatchException exception
        /// </summary>
        /// <param name="IncomingEdge">The IncomingEdge</param>
        /// <param name="currentNumOfParams">The current count of parameters</param>
        /// <param name="expectedNumOfParams">The expected count of parameters</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public EdgeParameterCountMismatchException(String edge, UInt32 currentNumOfParams, UInt32 expectedNumOfParams, Exception innerException = null)
			: base(innerException)
        {
            Edge = edge;
            CurrentNumOfParams = currentNumOfParams;
            ExpectedNumOfParams = expectedNumOfParams;
            _msg = String.Format("The edge [{0}] expects [{1}] params but found [{2}].", Edge, ExpectedNumOfParams, CurrentNumOfParams);
        }
        
    }
    
}
