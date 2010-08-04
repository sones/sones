/*
 * DBTypeOfAttributeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;
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
        public AEdgeType EdgeType { get; set; }

        public DBTypeOfAttributeDefinition()
        {
            TypeCharacteristics = new TypeCharacteristics();
        }

    }
}
