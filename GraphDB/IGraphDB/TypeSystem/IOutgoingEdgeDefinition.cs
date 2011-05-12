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
    /// An interface that represents an outgoing edge definition on a vertex type definition.
    /// </summary>
    public interface IOutgoingEdgeDefinition : IAttributeDefinition
    {
        /// <summary>
        /// The type of the edge. Never <c>NULL</c>.
        /// </summary>
        IEdgeType EdgeType { get; }

        /// <summary>
        /// The type of the inner edges of an multi edge. Might be <c>NULL</c>.
        /// </summary>
        IEdgeType InnerEdgeType { get; }

        /// <summary>
        /// The source vertex type. Never <c>NULL</c>.
        /// </summary>
        IVertexType SourceVertexType { get; }

        /// <summary>
        /// The target vertex type. Never <c>NULL</c>.
        /// </summary>
        IVertexType TargetVertexType { get; }

        /// <summary>
        /// The multiplicity of this outgoing edge definition.
        /// </summary>
        EdgeMultiplicity Multiplicity { get; }
    }
}
