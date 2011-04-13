using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type has a property without a type.
    /// </summary>
    public sealed class EmptyPropertyTypeException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates an instance of EmptyEdgeTypeException.
        /// </summary>
        /// <param name="myPredefinition">The predefinition that causes the exception.</param>
        /// <param name="myPropertyName"></param>
        public EmptyPropertyTypeException(VertexTypePredefinition myPredefinition, String myPropertyName)
        {
            Predefinition = myPredefinition;
            PropertyName = myPropertyName;
            _msg = string.Format("The property type {0} on vertex type {1} is empty.", myPropertyName, myPredefinition.VertexTypeName);
        }

        /// <summary>
        /// The predefinition that causes the exception.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The property that causes the exception.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
