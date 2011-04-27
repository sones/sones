using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.CreateVertexTypes
{
    public class IndexPredefinition
    {
        /// <summary>
        /// The name of the index type.
        /// </summary>
        public string TypeName { get; private set; }
        
        /// <summary>
        /// The name of the index
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Stores the names of the properties, that will be indexed.
        /// </summary>
        private HashSet<String> _properties = new HashSet<string>();

        /// <summary>
        /// The set of properties that will be indexed.
        /// </summary>
        public ISet<String> Properties { get { return _properties; } }

        /// <summary>
        /// The vertexTypeName that defines the index.
        /// </summary>
        public string VertexTypeName { get; set; }
        /// <summary>
        /// Creates a new instance of IndexPredefinition.
        /// </summary>
        public IndexPredefinition() { }

        /// <summary>
        /// Creates a new instance of IndexPredefinition.
        /// </summary>
        /// <param name="myName">The property that will be indexed.</param>
        public IndexPredefinition(String myName) 
        {
            Name = myName;
        }

        /// <summary>
        /// Adds a new property to this index pedefinition.
        /// </summary>
        /// <param name="myPropertyName">The name of the property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IndexPredefinition AddProperty(String myPropertyName)
        {
            _properties.Add(myPropertyName);
            return this;
        }

        /// <summary>
        /// Adds new properties to this index pedefinition.
        /// </summary>
        /// <param name="myPropertyNames">The name of the properties.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IndexPredefinition AddProperty(IEnumerable<String> myPropertyNames)
        {
            foreach (var propertyName in myPropertyNames)
                AddProperty(propertyName);

            return this;
        }

        /// <summary>
        /// Sets the type of the index.
        /// </summary>
        /// <param name="myIndexTypeName">The type name of the index.</param>
        public IndexPredefinition SetIndexType(String myIndexTypeName)
        {
            TypeName = myIndexTypeName;
            
            return this;
        }

        public IndexPredefinition SetVertexType(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;

            return this;
        }

    }
}
