using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_AttributeIsNotDefined : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        public Error_AttributeIsNotDefined(String myAttributeName)
        {
            AttributeName = myAttributeName;

        }

        public Error_AttributeIsNotDefined(String myType, String myAttributeName)
        {
            TypeName = myType;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            if (TypeName != null)
                return String.Format("The attribute \"{0}\" is not defined on type \"{1}\"!", AttributeName, TypeName);
            else
                return String.Format("The attribute \"{0}\" is not defined!", AttributeName);
        }


    }
}
