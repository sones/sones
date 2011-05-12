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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for vertex properties
    /// </summary>
    public sealed class PropertyPredefinition: AttributePredefinition
    {
        #region Data

        /// <summary>
        /// Should there be an index on the property?
        /// </summary>
        public Boolean IsIndexed { get; private set; }

        /// <summary>
        /// Should this property be mandatory?
        /// </summary>
        public Boolean IsMandatory { get; private set; }

        /// <summary>
        /// Should this property be unique?
        /// </summary>
        public Boolean IsUnique { get; private set; }

        /// <summary>
        /// The default value for this property.
        /// </summary>
        public String DefaultValue { get; private set; }

        /// <summary>
        /// The multiplicity of this property.
        /// </summary>
        public PropertyMultiplicity Multiplicity { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyPredefinition.
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        public PropertyPredefinition(String myPropertyName)
            : base(myPropertyName)
        {
            IsIndexed = false;
            IsMandatory = false;
            IsUnique = false;
            Multiplicity = PropertyMultiplicity.Single;
        }

        #endregion

        public PropertyPredefinition SetAsIndexed()
        {
            IsIndexed = true;

            return this;
        }

        public PropertyPredefinition SetAsMandatory()
        {
            IsMandatory = true;

            return this;
        }

        public PropertyPredefinition SetAsUnique()
        {
            IsUnique = true;

            return this;
        }

        public PropertyPredefinition SetMultiplicityToList()
        {
            Multiplicity = PropertyMultiplicity.List;

            return this;
        }

        public PropertyPredefinition SetMultiplicityToSet()
        {
            Multiplicity = PropertyMultiplicity.Set;

            return this;
        }


        public PropertyPredefinition SetDefaultValue(string myDefaultValue)
        {
            DefaultValue = myDefaultValue;
            return this;
        }

        public PropertyPredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        public PropertyPredefinition SetAttributeType(String myTypeName)
        {
            if (myTypeName != null)
                AttributeType = myTypeName;

            return this;
        }


    }
}