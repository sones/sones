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

using System.Linq;
using System.Collections.Generic;
using System;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The definition for an edge type.
    /// </summary>
    public sealed class EdgeTypePredefinition : ATypePredefinition
    {
        #region constructor

        public EdgeTypePredefinition(String myTypeName)
            :base (myTypeName, "Edge")
        {
        }

        #endregion

        #region fluent methods of abstract member

        /// <summary>
        /// Sets the name of the vertex type this one inherits from
        /// </summary>
        /// <param name="myComment">The name of the super vertex type</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition SetSuperVertexTypeName(String mySuperVertexTypeName)
        {
            if (!string.IsNullOrEmpty(mySuperVertexTypeName))
            {
                SuperTypeName = mySuperVertexTypeName;
            }
            return this;
        }

        /// <summary>
        /// Adds an unknown property to the vertex type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        {
            if (myUnknownPredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myUnknownPredefinition);
                _unknown++;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the vertex type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myPropertyDefinition);
                _properties++;
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _uniques = (_uniques) ?? new List<UniquePredefinition>();
                _uniques.Add(myUniqueDefinition);
            }

            return this;
        }

        /// <summary>
        /// Marks the vertex type as sealed.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition MarkAsSealed()
        {
            IsSealed = true;
            return this;
        }

        /// <summary>
        /// Marks the vertex type as abstract.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition MarkAsAbstract()
        {
            IsAbstract = true;
            return this;
        }

        /// <summary>
        /// Sets the comment of the vertex type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        #endregion
    }
}
