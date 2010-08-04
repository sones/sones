using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors.DBObjectErrors
{
    public class Error_UndefinedAttributeNotFound : GraphDBObjectError
    {
        public String UndefinedAttribute { get; private set; }
        
        public Error_UndefinedAttributeNotFound(String myUndefAttribute)
        {
            UndefinedAttribute = myUndefAttribute;
        }

        public override string ToString()
        {
            return String.Format("Undefined attribute \" {0} \" not found.", UndefinedAttribute);
        }
    
    }
}
