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

namespace sones.Library.Commons.Security
{
    /// <summary>
    /// The interface for the graph element authentication
    /// </summary>
    public interface IGraphElementAuthentication
    {
        /// <summary>
        /// Authentication for vertices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myToBeCheckedVertexID">The vertex that should be authenticated</param>
        /// <param name="myCorrespondingVertexTypeID">The vertex that should be authenticated</param>
        /// <param name="myWantedAction">The requested action</param>
        /// <returns>True for successful authentication, otherwise false</returns>
        Boolean Authenticate(SecurityToken mySecurityToken,
                             UInt64 myToBeCheckedVertexID,
                             UInt64 myCorrespondingVertexTypeID,
                             Right myWantedAction = Right.Traverse);
    }
}