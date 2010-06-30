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
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_IndexRebuildError : GraphDBIndexError
    {
        public GraphDBType Type { get; private set; }
        public ObjectLocation IndexLocation { get; private set; }
        
        public Error_IndexRebuildError(GraphDBType myType, ObjectLocation myObjectLocation)
        {
            Type = myType;
            IndexLocation = myObjectLocation;
        }

        public override string ToString()
        {
            return String.Format("Could not rebuild index of type \"{0}\" at ObjectLocation \"{1}\".", Type.Name, IndexLocation.Path);
        }
    }
}
