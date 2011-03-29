using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The data type of the SelectValueAssignment does not match the type
    /// </summary>
    public class SelectValueAssignmentDataTypeDoesNotMatchException : AGraphDBSelectException
    {
        public String ExpectedDataType { get; private set; }
        public String DataType { get; private set; }

        /// <summary>
        /// Creates a new SelectValueAssignmentDataTypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedDataType">The expected data type</param>
        /// <param name="myDataType">The current data type</param>
        public SelectValueAssignmentDataTypeDoesNotMatchException(String myExpectedDataType, String myDataType)
        {
            ExpectedDataType = myExpectedDataType;
            DataType = myDataType;
        }

        public override string ToString()
        {
            return String.Format("The data type of the SelectValueAssignment \"{0}\" does not match the type \"{1}\"!", DataType, ExpectedDataType);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.SelectValueAssignmentDataTypeDoesNotMatch; }
        }
    }
}
