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

/*
 * DBTypeOfAttributeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    /// <summary>
    /// This is the type definition of the type of an attribute. e.g. List&lt;String&gt;
    /// </summary>
    public class DBTypeOfAttributeDefinition
    {

        public KindsOfType Type { get; set; }

        /// <summary>
        /// The name of the type
        /// </summary>
        public String Name { get; set; }

        public TypeCharacteristics TypeCharacteristics { get; set; }
        public String EdgeType { get; set; }
        public EdgeTypeParamDefinition[] Parameters { get; set; }

        public DBTypeOfAttributeDefinition()
        {
            TypeCharacteristics = new TypeCharacteristics();
        }

    }
}
