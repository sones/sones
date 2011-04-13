using System;
using System.Collections.Generic;
using sones.Library.LanguageExtensions;

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
            _msg = String.Format("The vertex does not contain an undefined attribute with name \" {0} \".", AttrName);
        }

        /// <summary>
        /// Creates a new InvalidUndefinedVertexAttributesException exception
        /// </summary>
        /// <param name="myListOfAttrNames">A list of attribute names</param>
        public InvalidUndefinedVertexAttributesException(List<String> myListOfAttrNames)
        {
            ListOfAttrNames = myListOfAttrNames;
            String retVal = ListOfAttrNames[0];

            for (int i = 1; i < ListOfAttrNames.Count; i++)
                retVal += "," + ListOfAttrNames[i];

            _msg = String.Format("The object does not contains the undefined attributes \" {0} \".", retVal);
        }

    }
}
