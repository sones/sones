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
        /// or for properties that are directly connected to IVertices like UUID --> VertexID or Creation --> CreationDate
        /// <param name="aVertex">The vertex that needs to be consulted</param>
        /// <returns>The value as IComparable</returns>
        IComparable ExtractValue(IVertex aVertex);
    }
}
