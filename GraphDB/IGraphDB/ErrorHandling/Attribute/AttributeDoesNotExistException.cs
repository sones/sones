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


namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if an attribute is used, that does not exists on an specific vertex type or edge type.
    /// </summary>
    public class AttributeDoesNotExistException: AGraphDBAttributeException  
    {
        /// <summary>
        /// The name of the vertex type or edge type that should define the attribute.
        /// </summary>
        /// <remarks><c>NULL</c>, if unknown.</remarks>
        public string TypeName { get; private set; }

        /// <summary>
        /// The name of the attribute that does not exist.
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// Create a new instance of AttributeDoesNotExistException.
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute that does not exist.</param>
        /// <param name="myTypeName">The name of the vertex type or edge type that should define the attribute.</param>
        public AttributeDoesNotExistException(String myAttributeName, String myTypeName = null, String myInfo = "")
        {
            TypeName = myTypeName;
            AttributeName = myAttributeName;

            _msg = (myTypeName == null)
                ? String.Format("The attribute {0} does not exist. {1}", myAttributeName, myInfo)
                : String.Format("The attribute {1}.{0} does not exist. {2}", myAttributeName, myTypeName, myInfo);
        }

    }
}
