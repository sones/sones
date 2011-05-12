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
    /// Could not assign the value of the undefined attribute to an defined attribute of a certain type
    /// </summary>
    public sealed class InvalidUndefAttrTypeException : AGraphQLAttributeAssignmentException
    {        
        public String Attribute     { get; private set; }
        public String AttributeType { get; private set; }

        /// <summary>
        /// Creates a new InvalidUndefAttrTypeException exception
        /// </summary>
        /// <param name="myUndefAttribute">The undefined attribute</param>
        /// <param name="myAttributeType">The target attribute type</param>
        public InvalidUndefAttrTypeException(String myUndefAttribute, String myAttributeType)
        {            
            Attribute = myUndefAttribute;
            AttributeType = myAttributeType;
            _msg = String.Format("Could not assign the value of the undefined attribute \" {0} \" to an defined attribute \" {1} \" with type \" {2} \".", Attribute, Attribute, AttributeType);
        }
       
    }
}
