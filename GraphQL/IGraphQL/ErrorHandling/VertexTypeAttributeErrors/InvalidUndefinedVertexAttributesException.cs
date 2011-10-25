/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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

		/// <summary>
		/// Initializes a new instance of the InvalidUndefinedVertexAttributesException class using an attribute name.
		/// </summary>
		/// <param name="myAttrName"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidUndefinedVertexAttributesException(String myAttrName, Exception innerException = null) : base(innerException)
        {
            AttrName = myAttrName;
            _msg = String.Format("The vertex does not contain an undefined attribute with name \" {0} \".", AttrName);
        }

        /// <summary>
        /// Creates a new InvalidUndefinedVertexAttributesException exception using a list of attribute names.
        /// </summary>
        /// <param name="myListOfAttrNames">A list of attribute names</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidUndefinedVertexAttributesException(List<String> myListOfAttrNames, Exception innerException = null) : base(innerException)
        {
            ListOfAttrNames = myListOfAttrNames;
            String retVal = ListOfAttrNames[0];

            for (int i = 1; i < ListOfAttrNames.Count; i++)
                retVal += "," + ListOfAttrNames[i];

            _msg = String.Format("The object does not contains the undefined attributes \" {0} \".", retVal);
        }

    }
}
