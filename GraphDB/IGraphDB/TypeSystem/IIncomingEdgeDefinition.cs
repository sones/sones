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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents a definition of an incoming IncomingEdge.
    /// An incoming IncomingEdge is definied by the outgoing IncomingEdge that shares the same IncomingEdge.
    /// </summary>
    public interface IIncomingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The related outgoing IncomingEdge. Never <c>NULL</c>.
        /// </summary>
        IOutgoingEdgeDefinition RelatedEdgeDefinition { get; }
    }
}
