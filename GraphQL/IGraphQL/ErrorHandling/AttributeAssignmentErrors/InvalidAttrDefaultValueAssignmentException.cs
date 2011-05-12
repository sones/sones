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

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An assignment for an attribute from a certain type with a value of a second type is not valid
    /// </summary>
    public sealed class InvalidAttrDefaultValueAssignmentException : AGraphQLAttributeAssignmentException
    {
        #region data

        public String AttributeName { get; private set; }
        public String AttributeType { get; private set; }
        public String ExpectedType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a InvalidAttrDefaultValueAssignmentException exception
        /// </summary>
        /// <param name="myAttributeName">The attribute name</param>
        /// <param name="myAttrType">The type of the attribute</param>
        /// <param name="myExpectedType">The expected type of the attribute</param>
        public InvalidAttrDefaultValueAssignmentException(String myAttributeName, String myAttrType, String myExpectedType)
        {
            AttributeName = myAttributeName;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;                       
            _msg = String.Format("An assignment for the attribute \"{0}\" from type \"{1}\" with an value of the type \"{2}\" is not valid.", AttributeName, AttributeType, ExpectedType);            
        }

        /// <summary>
        /// Create a InvalidAttrDefaultValueAssignmentException exception
        /// </summary>
        /// <param name="myAttrType">The type of the attribute</param>
        /// <param name="myExpectedType">The expected type of the attribute</param>
        public InvalidAttrDefaultValueAssignmentException(String myAttrType, String myExpectedType)
        {            
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
            _msg = String.Format("Invalid type assignment for default value. Current type is \"{0}\". The type \"{1}\" is expected.", AttributeType, ExpectedType);
        }

        public InvalidAttrDefaultValueAssignmentException(String myInfo)
        {
            AttributeName = null;
            AttributeType = null;
            ExpectedType = null;

            _msg = myInfo;
        }

        #endregion
          
    }
}
