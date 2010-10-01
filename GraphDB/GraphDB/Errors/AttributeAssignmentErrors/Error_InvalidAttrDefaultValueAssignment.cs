/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttrDefaultValueAssignment : GraphDBAttributeAssignmentError
    {
        public String AttributeName { get; private set; }
        public String AttributeType   { get; private set; }
        public String ExpectedType  { get; private set; }

        public Error_InvalidAttrDefaultValueAssignment(String myAttributeName, String myAttrType, String myExpectedType)
        {
            AttributeName = myAttributeName;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        public Error_InvalidAttrDefaultValueAssignment(String myAttrType, String myExpectedType)
        {
            AttributeName = String.Empty;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        public override string ToString()
        {
            String retVal = String.Empty;

            if (AttributeName.Length > 0)
            {
                retVal = String.Format("An assignment for the attribute \"{0}\" from type \"{1}\" with an value of the type \"{2}\" is not valid.", AttributeName, AttributeType, ExpectedType);
            }
            else
            {
                retVal = String.Format("Invalid type assignment for default value. Current type is \"{0}\". The type \"{1}\" is expected.", AttributeType, ExpectedType);
            }

            return retVal;
        }
    }
}
