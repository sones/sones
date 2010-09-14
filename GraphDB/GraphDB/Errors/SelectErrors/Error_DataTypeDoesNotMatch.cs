using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SelectValueAssignmentDataTypeDoesNotMatch : GraphDBError
    {
        public String ExpectedDataType { get; private set; }
        public String DataType { get; private set; }

        public Error_SelectValueAssignmentDataTypeDoesNotMatch(String myExpectedDataType, String myDataType)
        {
            ExpectedDataType = myExpectedDataType;
            DataType = myDataType;
        }

        public override string ToString()
        {
            return String.Format("The datatype of the SelectValueAssignment \"{0}\" does not match the type \"{1}\"!", DataType, ExpectedDataType);
        }
    }
}
