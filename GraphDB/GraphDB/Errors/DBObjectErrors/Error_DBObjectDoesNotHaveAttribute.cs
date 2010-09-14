using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_DBObjectDoesNotHaveAttribute : GraphDBObjectError
    {
        public String AttributeName { get; private set; }

        public Error_DBObjectDoesNotHaveAttribute(String myAttributeName)
        {
            AttributeName = myAttributeName;

        }

        public override string ToString()
        {
            return String.Format("The dbObject does not have attribute \"{0}\"!", AttributeName);
        }
    }
}
