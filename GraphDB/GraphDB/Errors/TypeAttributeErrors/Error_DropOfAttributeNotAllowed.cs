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
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_DropOfAttributeNotAllowed : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }
        public Dictionary<TypeAttribute, GraphDBType> ConflictingAttributes { get; private set; }

        public Error_DropOfAttributeNotAllowed(String myType, String myAttributeName, Dictionary<TypeAttribute, GraphDBType> myConflictingAttributes)
        {
            ConflictingAttributes = myConflictingAttributes;
            TypeName = myType;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var aConflictingAttribute in ConflictingAttributes)
            {
                sb.Append(String.Format("{0} ({1}),", aConflictingAttribute.Key.Name, aConflictingAttribute.Value.Name));
            }

            sb.Remove(sb.Length - 1, 1);

            return String.Format("It is not possible to drop {0} of type {1} because there are remaining references from the following attributes: {2}" + Environment.NewLine + "Please remove them in previous.", AttributeName, TypeName, sb.ToString());

        }


    }
}
