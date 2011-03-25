using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class InvalidAttrDefaultValueAssignmentException : AGraphQLException
    {
        #region data

        public String AttributeName { get; private set; }
        public String AttributeType { get; private set; }
        public String ExpectedType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a InvalidAttrDefaultValueAssignmentException exception
        /// </summary>
        /// <param name="myAttributeName">The attribute name</param>
        /// <param name="myAttrType">The type of the attribute</param>
        /// <param name="myExpectedType">The expected type of the attribute</param>
        public InvalidAttrDefaultValueAssignmentException(String myAttributeName, String myAttrType, String myExpectedType)
        {
            AttributeName = myAttributeName;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        /// <summary>
        /// Create a InvalidAttrDefaultValueAssignmentException exception
        /// </summary>
        /// <param name="myAttrType">The type of the attribute</param>
        /// <param name="myExpectedType">The expected type of the attribute</param>
        public InvalidAttrDefaultValueAssignmentException(String myAttrType, String myExpectedType)
        {
            AttributeName = String.Empty;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        #endregion

        public override string ToString()
        {
            String retVal = String.Empty;

            if (AttributeName.Length > 0)
            {
                retVal = String.Format("An assignment for the attribute \"{0}\" from type \"{1}\" with an value of the type \"{2}\" is not valid.", AttributeName, AttributeType, ExpectedType);
            }
            else
            {
                retVal = String.Format("Invalid type assignment for default value. Current type is \"{0}\". The type \"{1}\" is expected.", AttributeType, ExpectedType);
            }

            return retVal;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidAttrDefaultValueAssignment; }
        }  
    }
}
