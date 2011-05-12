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

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// This is the type definition of the type of an attribute. e.g. List&lt;String&gt;
    /// </summary>
    public sealed class DBTypeOfAttributeDefinition
    {
        /// <summary>
        /// The name of the type
        /// </summary>
        public String Name { get; set; }

        public String Type { get; set; }

        public TypeCharacteristics TypeCharacteristics { get; set; }
        public String EdgeType { get; set; }

        public DBTypeOfAttributeDefinition()
        {
            TypeCharacteristics = new TypeCharacteristics();
        }
    }
}
