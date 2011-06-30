/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.CreateVertexTypes
{
    public class IndexPredefinition
    {
        public string Edition { get; private set; }

        /// <summary>
        /// The name of the index type.
        /// </summary>
        public string TypeName { get; private set; }
        
        /// <summary>
        /// The name of the index
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The options that will be passed to the index instance
        /// </summary>
        public Dictionary<String, Object> IndexOptions { get; private set; }

        /// <summary>
        /// Stores the names of the properties, that will be indexed.
        /// </summary>
        private List<String> _properties = new List<string>();

        /// <summary>
        /// The set of properties that will be indexed.
        /// </summary>
        public IList<String> Properties { get { return _properties.AsReadOnly(); } }

        /// <summary>
        /// The vertexTypeName that defines the index.
        /// </summary>
        public string VertexTypeName { get; private set; }
        /// <summary>
        /// Creates a new instance of IndexPredefinition.
        /// </summary>
        public IndexPredefinition() { }

        public string Comment { get; private set; }

        /// <summary>
        /// Creates a new instance of IndexPredefinition.
        /// </summary>
        /// <param name="myName">The name of the index.</param>
        public IndexPredefinition(String myName) 
        {
            Name = myName;
        }

        /// <summary>
        /// Adds an option to the index predefinition
        /// </summary>
        /// <param name="myOptionKey">The name of the option</param>
        /// <param name="myOptionValue">The value of the option</param>
        /// <returns>The updated IndexPredefinition</returns>
        public IndexPredefinition AddOption(String myOptionKey, Object myOptionValue)
        {
            if (IndexOptions == null)
            {
                IndexOptions = new Dictionary<string, object>();
            }

            if (IndexOptions.ContainsKey(myOptionKey))
            {
                IndexOptions[myOptionKey] = myOptionValue;
            }
            else
            {
                IndexOptions.Add(myOptionKey, myOptionValue);
            }

            return this;
        }

        /// <summary>
        /// Add options to the index predefinition
        /// </summary>
        /// <param name="myOptions">The options that should be set</param>
        /// <returns>The updated IndexPredefinition</returns>
        public IndexPredefinition AddOptions(Dictionary<String, Object> myOptions)
        {
            if (IndexOptions == null)
            {
                IndexOptions = new Dictionary<string, object>();
            }

            foreach (var aOption in myOptions)
            {
                AddOption(aOption.Key, aOption.Value);
            }

            return this;
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

        public IndexPredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }


        public IndexPredefinition SetEdition(string myEdition)
        {
            Edition = myEdition;

            return this;
        }

    }
}
