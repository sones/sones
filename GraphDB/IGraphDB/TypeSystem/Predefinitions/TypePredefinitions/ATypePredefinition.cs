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
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.TypeSystem
{
    public abstract class ATypePredefinition
    {
        #region Data

        // The name of the type that is going to be created
        public readonly string TypeName;
        protected List<AAttributePredefinition> _attributes;
        
        protected int _properties = 0;
        protected int _unknown = 0;

        public int AttributeCount 
        {
            get { return (_attributes == null) ? 0 : _attributes.Count; }
        }
        
        public int PropertyCount
        {
            get { return _properties; }
        }
        
        public int UnknownPropertyCount
        {
            get { return _unknown; }
        }

        /// <summary>
        /// The name of the type this types inherites from.
        /// </summary>
        public string SuperTypeName { get; protected set; }

        /// <summary>
        /// The properties of the type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> Properties
        {
            get { return (_attributes== null) ? null : _attributes.OfType<PropertyPredefinition>(); }
        }

        /// <summary>
        /// the unknown attributes of this type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> UnknownAttributes
        {
            get { return (_attributes == null) ? null : _attributes.OfType<UnknownAttributePredefinition>(); }
        }

        /// <summary>
        /// Gets if the type will be sealed.
        /// </summary>
        public bool IsSealed { get; protected set; }

        /// <summary>
        /// Gets the comment for this type.
        /// </summary>
        public string Comment { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new type definition.
        /// </summary>
        /// <param name="myTypeName">The name of the type.</param>
        /// <param name="mySuperTypeName">The name of the parent type.</param>
        public ATypePredefinition(String myTypeName, String mySuperTypeName)
        {
            if (string.IsNullOrEmpty(myTypeName))
                throw new EmptyTypeNameException();

            TypeName = myTypeName;
            SuperTypeName = mySuperTypeName;
            IsSealed = false;
            Comment = String.Empty;
        }

        #endregion

        #region fluent methods
        //Note: all fluent methods are declared in the specializing classes (EdgeTypePredefinition & VertexTypePredefinition)
        //      because of the fluent interface it is impossible to declare them inside the abstract class !
        #endregion

        #region public methods

        /// <summary>
        /// Removes all unknown attributes.
        /// </summary>
        /// <remarks>This method is used internally.</remarks>
        public void ResetUnknown()
        {
            _attributes.RemoveAll(x => x is UnknownAttributePredefinition);
            _unknown = 0;
        }

        #endregion
    }
}
