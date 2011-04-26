using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.CreateVertexTypes
{
    /// <summary>
    /// A class that represents a set of properties, that must be unique.
    /// </summary>
    public class UniquePredefinition
    {
        /// <summary>
        /// Stores the names of the properties, that will be unique together.
        /// </summary>
        private HashSet<String> _properties = new HashSet<string>();

        /// <summary>
        /// The set of properties that will be unique together.
        /// </summary>
        public ISet<String> Properties { get { return _properties; } }

        /// <summary>
        /// Creates a new instance of UniquePredefinition.
        /// </summary>
        public UniquePredefinition() { }

        /// <summary>
        /// Creates a new instance of UniquePredefinition.
        /// </summary>
        /// <param name="myProperty">The property that will be unique.</param>
        /// <remarks>Same as <code>new UniquePredefinition().AddProperty(myProperty)</code>.</remarks>
        public UniquePredefinition(String myProperty) 
        {
            AddPropery(myProperty);
        }

        /// <summary>
        /// Adds a new property to this unique pedefinition.
        /// </summary>
        /// <param name="myPropertyName">The name of the property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public UniquePredefinition AddPropery(String myPropertyName)
        {
            _properties.Add(myPropertyName);
            return this;
        }

        /// <summary>
        /// Adds new properties to this unique pedefinition.
        /// </summary>
        /// <param name="myPropertyNames">The name of the properties.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public UniquePredefinition AddPropery(IEnumerable<String> myPropertyNames)
        {
            foreach (var propertyName in myPropertyNames)
                AddPropery(propertyName);

            return this;
        }

    }
}
