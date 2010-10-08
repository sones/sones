using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_AttributeExistsInSupertype : GraphDBAttributeError
    {
        public String Attribute { get; private set; }
        public String Supertype { get; private set; }

        public Error_AttributeExistsInSupertype(String myAttribute)
        {
            Attribute = myAttribute;
        }

        public Error_AttributeExistsInSupertype(String myAttribute, String mySupertype)
        {
            Attribute = myAttribute;
            Supertype = mySupertype;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" already exists in supertype \"{1}\"!", Attribute, Supertype);
        }
    }
}
