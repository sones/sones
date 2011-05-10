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

        public AttributeType Kind { get { return AttributeType.BinaryProperty; } }

        public IBaseType RelatedType { get; internal set; }

        public bool IsUserDefined { get; internal set; }

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.ID == ID;
        }

        #endregion

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAttributeDefinition);
        }

    }
}
