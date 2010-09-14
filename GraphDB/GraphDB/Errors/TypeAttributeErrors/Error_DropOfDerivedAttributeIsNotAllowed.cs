using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_DropOfDerivedAttributeIsNotAllowed : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        public Error_DropOfDerivedAttributeIsNotAllowed(String myType, String myAttributeName)
        {
            TypeName = myType;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {

            return String.Format("Due to the attribute \"{0}\" is derived from type \"{1}\" you can not drop it!", AttributeName, TypeName);
        }


    }
}
