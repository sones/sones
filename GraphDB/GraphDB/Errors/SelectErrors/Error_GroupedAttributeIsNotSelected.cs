using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_GroupedAttributeIsNotSelected : GraphDBSelectError
    {
        public TypeAttribute TypeAttribute { get; private set; }

        public Error_GroupedAttributeIsNotSelected(TypeAttribute myTypeAttribute)
        {
            TypeAttribute = myTypeAttribute;
        }

        public override string ToString()
        {
            return String.Format("The attribute '{0}' is not selected and can't be grouped.", TypeAttribute.Name);
        }
    }
}
