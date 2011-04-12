using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The datatype does not match the type
    /// </summary>
    public sealed class DataTypeDoesNotMatchException : AGraphQLException
    {
        public String ExpectedDataType { get; private set; }
        public String DataType { get; private set; }

        /// <summary>
        /// Creates a new DataTypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedDataType">The expected data type</param>
        /// <param name="myDataType">The current data type</param>
        public DataTypeDoesNotMatchException(String myExpectedDataType, String myDataType)
        {
            ExpectedDataType = myExpectedDataType;
            DataType = myDataType;
        }

        public override string ToString()
        {
            return String.Format("The datatype \"{0}\" does not match the type \"{1}\"!", DataType, ExpectedDataType);
        }
               
    }
}
