/* <id name="GraphDB – AttributeUpdateOrAssign" />
 * <copyright file="AttributeUpdateOrAssign.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    public class UndefinedAttributeDefinition
    {
        public String AttributeName { get; set; }
        public IObject AttributeValue { get; set; }
    }

}
