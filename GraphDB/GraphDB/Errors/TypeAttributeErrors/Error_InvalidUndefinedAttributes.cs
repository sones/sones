using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidUndefinedAttributes : GraphDBAttributeError
    {
        public String AttrName { get; private set; }
        public List<String> ListOfAttrNames { get; private set; }

        public Error_InvalidUndefinedAttributes(String myAttrName)
        {
            AttrName = myAttrName;
        }

        public Error_InvalidUndefinedAttributes(List<String> myListOfAttrNames)
        {
            ListOfAttrNames = myListOfAttrNames;
        }

        public override string ToString()
        {
            if (ListOfAttrNames.IsNullOrEmpty())
            {
                return String.Format("The object does not contain an undefined attribute with name \" {0} \".", AttrName);
            }
            else
            {
                String retVal = ListOfAttrNames[0];

                for (int i = 1; i < ListOfAttrNames.Count; i++)
                    retVal += "," + ListOfAttrNames[i];

                return String.Format("The object does not contains the undefined attributes \" {0} \".", retVal);
            }
        }
    
    }
}
