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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class represents a property definition
    /// </summary>
    internal class PropertyDefinition: IPropertyDefinition
    {

        #region IPropertyDefinition Members

        public bool IsMandatory { get; internal set; }

        public bool IsUserDefinedType { get; internal set; }

        public Type BaseType { get; internal set; }

        public PropertyMultiplicity Multiplicity { get; internal set; }

        public IEnumerable<IIndexDefinition> InIndices { get; internal set; }

        public IComparable DefaultValue { get; internal set; }

        public bool IsUserDefined { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public long ID { get; internal set; }

        public string Name { get; internal set; }

        public AttributeType Kind { get { return AttributeType.Property; } }

        public IBaseType RelatedType { get; internal set; }

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
