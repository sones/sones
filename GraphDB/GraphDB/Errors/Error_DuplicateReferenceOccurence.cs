/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Errors
{
    public class Error_DuplicateReferenceOccurence : GraphDBError
    {
        public String Type { get; private set; }

        public Error_DuplicateReferenceOccurence(TypeReferenceDefinition myType)
        {
            Type = myType.ToString();
        }

        public Error_DuplicateReferenceOccurence(String myType)
        {
            Type = myType;
        }

        public override string ToString()
        {
            return String.Format("There is already a reference for type \"{0}\"!", Type);
        }
    }
}
