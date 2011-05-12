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
    /// The timeout of a query has been reached
    /// </summary>
    public sealed class SelectTimeOutException : AGraphQLSelectException
    {
        public Int64 TimeOut { get; private set; }

        /// <summary>
        /// Creates a new SelectTimeOutException exception
        /// </summary>
        /// <param name="myTimeout">The timeout</param>
        public SelectTimeOutException(Int64 myTimeout)
        {
            TimeOut = myTimeout;
            _msg = String.Format("Aborting query because the timeout of {0}ms has been reached.", TimeOut);
        }

    }
}
