using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Errors
{
    public class Error_DuplicateReferenceOccurence : GraphDBError
    {
        public String Type { get; private set; }

        public Error_DuplicateReferenceOccurence(TypeReferenceDefinition myType)
        {
            Type = myType.ToString();
        }

        public Error_DuplicateReferenceOccurence(String myType)
        {
            Type = myType;
        }

        public override string ToString()
        {
            return String.Format("There is already a reference for type \"{0}\"!", Type);
        }
    }
}
