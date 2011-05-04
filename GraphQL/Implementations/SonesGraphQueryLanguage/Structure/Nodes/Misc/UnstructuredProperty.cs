using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.GQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This class represents an unstructured property
    /// </summary>
    public sealed class UnstructuredProperty : IAttributeDefinition, IEquatable<UnstructuredProperty>
    {
        #region Data

        /// <summary>
        /// The name of the unstructured property
        /// </summary>
        private readonly string _name;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new unstructured property
        /// </summary>
        /// <param name="myUnstructuredPropertyName">The name of the unstructured property</param>
        public UnstructuredProperty(string myUnstructuredPropertyName)
        {
            _name = myUnstructuredPropertyName;
        }

        #endregion


        #region IAttributeDefinition Members

        public string Name
        {
            get { return _name; }
        }

        public long ID
        {
            get { return Int64.MaxValue; }
        }

        public AttributeType Kind
        {
            get { throw new NotImplementedException(); }
        }

        public IBaseType RelatedType
        {
            get { return null; }
        }

        public bool IsUserDefined
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEquatable<UnstructuredProperty> Members

        public bool Equals(UnstructuredProperty other)
        {
            return (other != null) && (_name == other.Name);
        }

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition other)
        {
            if (other is UnstructuredProperty)
                return Equals(other as UnstructuredProperty);

            return false;
        }

        #endregion

    }
}
