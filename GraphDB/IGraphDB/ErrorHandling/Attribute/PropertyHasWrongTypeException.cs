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


namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a property is set with a value of an unexpected type.
    /// </summary>
    public class PropertyHasWrongTypeException : AGraphDBAttributeException  
    {
        //private string p;
        //private string p_2;
        //private TypeSystem.PropertyMultiplicity propertyMultiplicity;
        //private string p_3;
        //private string p_4;

        /// <summary>
        /// The type that defines the property.
        /// </summary>
        public string DefiningTypeName { get; set; }

        /// <summary>
        /// The property that was used with the wrong type.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The expected type name.
        /// </summary>
        public string ExpectedTypeName { get; private set; }

        /// <summary>
        /// The type name of the value.
        /// </summary>
        public string UnexpectedTypeName { get; private set; }

        /// <summary>
        /// Creates an instance of PropertyHasWrongTypeException.
        /// </summary>
        /// <param name="myDefiningTypeName">The type that defines the property.</param>
        /// <param name="myPropertyName">The property that was used with the wrong type.</param>
        /// <param name="myExpectedTypeName">The expected type name.</param>
        /// <param name="myUnexpectedTypeName">The type name of the value.</param>
        public PropertyHasWrongTypeException(String myDefiningTypeName, String myPropertyName, String myExpectedTypeName, String myUnexpectedTypeName)
        {
            DefiningTypeName = myDefiningTypeName;
            PropertyName = myPropertyName;
            ExpectedTypeName = myExpectedTypeName;
            UnexpectedTypeName = myUnexpectedTypeName;
            _msg = string.Format("The property {0}.[1} needs a value of type {2} but was used with a value of type {3}", myDefiningTypeName, myPropertyName, myExpectedTypeName, myUnexpectedTypeName);
        }

        public PropertyHasWrongTypeException(String myDefiningTypeName, String myPropertyName, PropertyMultiplicity myMultiplicity, String myExpectedTypeName)
        {
            DefiningTypeName = myDefiningTypeName;
            PropertyName = myPropertyName;
            ExpectedTypeName = myExpectedTypeName;
            _msg = string.Format("The property {0}.[1} needs a {3} of type {2} but was no {3}", myDefiningTypeName, myPropertyName, myExpectedTypeName, myMultiplicity.ToString());
        }
    }
}
