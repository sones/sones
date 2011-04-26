using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a property has type, that is unknown.
    /// </summary>
    public sealed class UnknownPropertyTypeException: AGraphDBVertexAttributeException
    {
        private VertexTypePredefinition Predefinition;
        private string PropertyName;

        public UnknownPropertyTypeException(VertexTypePredefinition myVertexTypeDefinition, string myPropertyName)
        {
            this.Predefinition = myVertexTypeDefinition;
            this.PropertyName = myPropertyName;
            _msg = string.Format("The property {0} on vertex type {1} has an unknown type.", myPropertyName, myVertexTypeDefinition.VertexTypeName);
        }
        
    }
}
