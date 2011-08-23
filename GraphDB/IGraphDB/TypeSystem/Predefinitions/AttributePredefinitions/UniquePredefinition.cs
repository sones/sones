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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// A class that represents a set of properties, that must be unique.
    /// </summary>
    public sealed class UniquePredefinition
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
