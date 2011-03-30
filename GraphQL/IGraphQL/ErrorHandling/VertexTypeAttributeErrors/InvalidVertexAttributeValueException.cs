using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The attribute has an invalid value
    /// </summary>
    public sealed class InvalidVertexAttributeValueException : AGraphQLVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public Object AttributeValue { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexAttributeValueException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        /// <param name="myAttributeValue">The value of the attribute</param>
        public InvalidVertexAttributeValueException(String myAttributeName, Object myAttributeValue)
        {
            AttributeName = myAttributeName;
            AttributeValue = myAttributeValue;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" has an invalid value: \"{1}\"", AttributeName, AttributeValue);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidVertexAttributeValue; }
        } 
    }
}
