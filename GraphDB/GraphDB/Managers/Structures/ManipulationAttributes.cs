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
 * ManipulationAttributes
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;



using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers
{

    public class ManipulationAttributes
    {
        /// <summary>
        /// dictionary of attribute assignments
        /// </summary>
        public Dictionary<TypeAndAttributeDefinition, IObject> Attributes { get; set; }

        /// <summary>
        /// mandatory attributes in the current insert statement
        /// </summary>
        public HashSet<AttributeUUID> MandatoryAttributes { get; set; }
        public List<AAttributeAssignOrUpdateOrRemove> AttributeToUpdateOrAssign { get; set; }
        public Dictionary<String, IObject> UndefinedAttributes { get; set; }
        public Dictionary<ASpecialTypeAttribute, Object> SpecialTypeAttributes { get; set; }

        public ManipulationAttributes()
        {
            Attributes = new Dictionary<TypeAndAttributeDefinition, IObject>();
            MandatoryAttributes = new HashSet<AttributeUUID>();
            AttributeToUpdateOrAssign = new List<AAttributeAssignOrUpdateOrRemove>();
            UndefinedAttributes = new Dictionary<string, IObject>();
            SpecialTypeAttributes = new Dictionary<ASpecialTypeAttribute, object>();
        }
    }

}
