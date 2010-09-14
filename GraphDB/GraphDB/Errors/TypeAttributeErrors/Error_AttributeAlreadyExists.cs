using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_AttributeAlreadyExists : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public GraphDBType Type { get; private set; }

        public Error_AttributeAlreadyExists(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public Error_AttributeAlreadyExists(GraphDBType myType, String myAttributeName)
        {
            Type = myType;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            if (Type != null)
                return String.Format("The attribute \"{0}\" already exist in type \"{1}\"!", AttributeName, Type.ToString());
            else
                return String.Format("The attribute \"{0}\" already exist!", AttributeName);
        }

    }
}
