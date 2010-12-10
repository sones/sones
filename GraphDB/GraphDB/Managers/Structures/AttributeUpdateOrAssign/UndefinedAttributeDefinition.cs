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

    /// <summary>
    /// The definition class for undefined attributes
    /// </summary>
    public class UndefinedAttributeDefinition
    {
        /// <summary>
        /// The undefined attribute name
        /// </summary>
        public String AttributeName { get; set; }
        /// <summary>
        /// The undefined attribute value
        /// </summary>
        public IObject AttributeValue { get; set; }
    }

}
