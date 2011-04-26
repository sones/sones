using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a property is set with a value of an unexpected type.
    /// </summary>
    public class PropertyHasWrongTypeException : AGraphDBAttributeException  
    {
        private string p;
        private string p_2;

        /// <summary>
        /// The type that defines the property.
        /// </summary>
        public string DefiningTypeName { get; set; }

        /// <summary>
        /// The property that was used with the wrong type.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The expected type name.
        /// </summary>
        public string ExpectedTypeName { get; private set; }

        /// <summary>
        /// The type name of the value.
        /// </summary>
        public string UnexpectedTypeName { get; private set; }

        /// <summary>
        /// Creates an instance of PropertyHasWrongTypeException.
        /// </summary>
        /// <param name="myDefiningTypeName">The type that defines the property.</param>
        /// <param name="myPropertyName">The property that was used with the wrong type.</param>
        /// <param name="myExpectedTypeName">The expected type name.</param>
        /// <param name="myUnexpectedTypeName">The type name of the value.</param>
        public PropertyHasWrongTypeException(String myDefiningTypeName, String myPropertyName, String myExpectedTypeName, String myUnexpectedTypeName)
        {
            DefiningTypeName = myDefiningTypeName;
            PropertyName = myPropertyName;
            ExpectedTypeName = myExpectedTypeName;
            UnexpectedTypeName = myUnexpectedTypeName;
            _msg = string.Format("The property {0}.[1} need a value of type {2} but was used with a value of type {3}", myDefiningTypeName, myPropertyName, myExpectedTypeName, myUnexpectedTypeName);
        }
    }
}
