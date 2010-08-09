/*
 * ManipulationAttributes
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;


using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Managers
{

    public class ManipulationAttributes
    {
        /// <summary>
        /// dictionary of attribute assignments
        /// </summary>
        public Dictionary<TypeAndAttributeDefinition, AObject> Attributes { get; set; }

        /// <summary>
        /// mandatory attributes in the current insert statement
        /// </summary>
        public HashSet<AttributeUUID> MandatoryAttributes { get; set; }
        public List<AAttributeAssignOrUpdateOrRemove> AttributeToUpdateOrAssign { get; set; }
        public Dictionary<String, AObject> UndefinedAttributes { get; set; }
        public Dictionary<ASpecialTypeAttribute, Object> SpecialTypeAttributes { get; set; }

        public ManipulationAttributes()
        {
            Attributes = new Dictionary<TypeAndAttributeDefinition, AObject>();
            MandatoryAttributes = new HashSet<AttributeUUID>();
            AttributeToUpdateOrAssign = new List<AAttributeAssignOrUpdateOrRemove>();
            UndefinedAttributes = new Dictionary<string, AObject>();
            SpecialTypeAttributes = new Dictionary<ASpecialTypeAttribute, object>();
        }
    }

}
