using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_UpdateAttributeValue : GraphDBAttributeError
    {
        public TypeAttribute Attribute { get; private set; }

        public Error_UpdateAttributeValue(TypeAttribute myAttribute)
        {
            Attribute = myAttribute;
        }

        public override string ToString()
        {
            return String.Format("Could not update value for attribute \"{0}\".", Attribute.Name);
        }
    }
}
