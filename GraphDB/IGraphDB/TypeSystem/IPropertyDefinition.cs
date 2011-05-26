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
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents a property definition on a vertex type definition.
    /// </summary>
    public interface IPropertyDefinition : IAttributeDefinition
    {
        /// <summary>
        /// Gets whether this property is mandatory.
        /// </summary>
        Boolean IsMandatory { get; }

        /// <summary>
        /// Gets whether this property type is user defined.
        /// </summary>
        Boolean IsUserDefinedType { get; }

        /// <summary>
        /// Gets the type of the property. This is always a c# value type.
        /// </summary>
        Type BaseType { get; }

        /// <summary>
        /// The multiplicity of this property.
        /// </summary>
        PropertyMultiplicity Multiplicity { get; }

        /// <summary>
        /// The default value for this property.
        /// </summary>
        IComparable DefaultValue { get; }

        /// <summary>
        /// Returns the list of index definitions the property is involved.
        /// </summary>
        IEnumerable<IIndexDefinition> InIndices { get; }

        /// <summary>
        /// Extracts the this property from a given vertex...
        /// </summary>
        /// With this method it is possible to create several PropertyDefinitions for things like usual properties (Age, Name, etc...)
        /// or for properties that are directly connected to IVertices like VertexID --> VertexID or Creation --> CreationDate
        /// <param name="aVertex">The vertex that needs to be consulted</param>
        /// <returns>The value as IComparable</returns>
        IComparable ExtractValue(IVertex aVertex);
    }
}
