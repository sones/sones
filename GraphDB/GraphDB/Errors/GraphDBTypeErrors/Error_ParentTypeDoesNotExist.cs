using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ParentTypeDoesNotExist : GraphDBTypeError
    {
        public String ParentType { get; private set; }
        public String Type { get; private set; }

        public Error_ParentTypeDoesNotExist(String myParentType, String myType)
        {
            ParentType = myParentType;
            Type = myType;
        }

        public override string ToString()
        {
            return String.Format("The parent type {0} of the type {1} does not exist.", ParentType, Type);
        }
    }
}
