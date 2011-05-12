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
