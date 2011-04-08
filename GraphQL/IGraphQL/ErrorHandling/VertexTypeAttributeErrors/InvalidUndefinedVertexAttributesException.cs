using System;
using System.Collections.Generic;
using sones.Library.LanguageExtensions;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The vertex does not contain an undefined attribute with this name
    /// </summary>
    public sealed class InvalidUndefinedVertexAttributesException : AGraphQLVertexAttributeException
    {
        public String AttrName { get; private set; }
        public List<String> ListOfAttrNames { get; private set; }

        public InvalidUndefinedVertexAttributesException(String myAttrName)
        {
            AttrName = myAttrName;
        }

        /// <summary>
        /// Creates a new InvalidUndefinedVertexAttributesException exception
        /// </summary>
        /// <param name="myListOfAttrNames">A list of attribute names</param>
        public InvalidUndefinedVertexAttributesException(List<String> myListOfAttrNames)
        {
            ListOfAttrNames = myListOfAttrNames;
        }

        public override string ToString()
        {
            if (ListOfAttrNames.IsNullOrEmpty())
            {
                return String.Format("The vertex does not contain an undefined attribute with name \" {0} \".", AttrName);
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
