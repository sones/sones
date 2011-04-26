using System;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class represents a binary property definition.
    /// </summary>
    internal class BinaryPropertyDefinition: IBinaryPropertyDefinition
    {
        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.BinaryProperty; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion
    }
}
