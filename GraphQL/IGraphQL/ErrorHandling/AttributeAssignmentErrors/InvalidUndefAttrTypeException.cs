using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Could not assign the value of the undefined attribute to an defined attribute of a certain type
    /// </summary>
    public sealed class InvalidUndefAttrTypeException : AGraphQLAttributeAssignmentException
    {        
        public String Attribute     { get; private set; }
        public String AttributeType { get; private set; }

        /// <summary>
        /// Creates a new InvalidUndefAttrTypeException exception
        /// </summary>
        /// <param name="myUndefAttribute">The undefined attribute</param>
        /// <param name="myAttributeType">The target attribute type</param>
        public InvalidUndefAttrTypeException(String myUndefAttribute, String myAttributeType)
        {            
            Attribute = myUndefAttribute;
            AttributeType = myAttributeType;
        }

        public override string ToString()
        {
            return String.Format("Could not assign the value of the undefined attribute \" {0} \" to an defined attribute \" {1} \" with type \" {2} \".", Attribute, Attribute, AttributeType);
        } 

    }
}
