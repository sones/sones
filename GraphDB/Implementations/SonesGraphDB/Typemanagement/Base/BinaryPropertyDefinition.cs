using System;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class represents a binary property definition.
    /// </summary>
    internal class BinaryPropertyDefinition: IBinaryPropertyDefinition
    {
        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long ID { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.BinaryProperty; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.AttributeID == AttributeID && EqualityComparer<IBaseType>.Default.Equals(RelatedType, myOther.RelatedType);
        }

        #endregion

    }
}
