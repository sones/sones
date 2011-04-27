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
    public sealed class UnstructuredProperty : IAttributeDefinition
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

        public long AttributeID
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

        #endregion
    }
}
