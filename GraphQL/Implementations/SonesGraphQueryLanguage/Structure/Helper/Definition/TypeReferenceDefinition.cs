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
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed  class TypeReferenceDefinition
    {

        public TypeReferenceDefinition(string typeName, string reference)
        {
            TypeName = typeName;
            Reference = reference;
        }

        public String TypeName { get; private set; }
        public String Reference { get; private set; }

        public override int GetHashCode()
        {
            return TypeName.GetHashCode() ^ Reference.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj as TypeReferenceDefinition == null)
            {
                return false;
            }
            return TypeName.Equals((obj as TypeReferenceDefinition).TypeName) && Reference.Equals((obj as TypeReferenceDefinition).Reference);
        }

    }
}
