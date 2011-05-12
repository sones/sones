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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The sub type of all 
    /// </summary>
    public interface IAttributeDefinition: IEquatable<IAttributeDefinition>
    {
        /// <summary>
        /// The Vertex ID of the vertex that represents this attribute.
        /// </summary>
        Int64 ID { get; }

        /// <summary>
        /// The name, that is unique for all attributes on a vertex type.
        /// </summary>
        String Name { get; }
        
        /// <summary>
        /// Returns if the attribute was created by the user.
        /// </summary>
        bool IsUserDefined { get; }
        /// <summary>
        /// The type of the attribute        
        /// </summary>
        /// <remarks>
        /// If Kind is <c>Property</c> you can cast this IAttributeDefinition to IPropertyDefinition.
        /// If Kind is <c>IncomingEdge</c> you can cast this IAttributeDefinition to IEdgeDefintiton.
        /// If Kind is <c>OutgoingEdge</c> you can cast this IAttributeDefinition to IEdgeDefinition.
        /// </remarks>
        AttributeType Kind { get; }

        /// <summary>
        /// The type that defines the attribute.
        /// </summary>
        IBaseType RelatedType { get; }
    }
}
