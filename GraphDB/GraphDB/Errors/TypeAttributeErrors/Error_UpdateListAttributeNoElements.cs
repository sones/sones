using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_UpdateAttributeNoElements : GraphDBAttributeError
    {
        public TypeAttribute Attribute { get; private set; }

        public Error_UpdateAttributeNoElements(TypeAttribute myAttribute)
        {
            Attribute = myAttribute;
        }

        public override string ToString()
        {
            return String.Format("Could not find any objects while adding or removing elements to the list attribute {0}.", Attribute);
        }
    }
}
