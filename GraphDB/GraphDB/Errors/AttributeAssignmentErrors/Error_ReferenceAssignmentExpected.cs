using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_ReferenceAssignmentExpected : GraphDBAttributeAssignmentError
    {
        public TypeAttribute TypeAttribute { get; private set; }
        public String Info { get; private set; }


        public Error_ReferenceAssignmentExpected(TypeAttribute typeAttribute)
        {
            TypeAttribute = typeAttribute;
            Message = String.Format("The attribute [{0}] expects a Reference assignment!", TypeAttribute.Name);
        }

        public override string ToString()
        {
            return String.Format("The attribute [{0}] expects a Reference assignment!", TypeAttribute.Name);
        }
    }
}
