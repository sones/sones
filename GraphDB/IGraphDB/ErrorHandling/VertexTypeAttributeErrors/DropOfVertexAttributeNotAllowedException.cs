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
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Droping a vertex attribute is not allowed, because of remaining references from other attributes
    /// </summary>
    public sealed class DropOfVertexAttributeNotAllowedException : AGraphDBVertexAttributeException
    {
        #region data

        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }
        public Dictionary<String, String> ConflictingAttributes { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new DropOfVertexAttributeNotAllowedException exception
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type</param>
        /// <param name="myVertexAttributeName">The name of the vertex attribute </param>
        /// <param name="myConflictingAttributes">A dictionary of the conflicting attributes (TypeAttributeName, GraphDBType)   </param>
        public DropOfVertexAttributeNotAllowedException(String myVertexTypeName, String myVertexAttributeName, Dictionary<String, String> myConflictingAttributes)
        {
            ConflictingAttributes = myConflictingAttributes;
            VertexTypeName = myVertexTypeName;
            VertexAttributeName = myVertexAttributeName;

            
            StringBuilder sb = new StringBuilder();

            foreach (var aConflictingAttribute in ConflictingAttributes)
            {
                sb.Append(String.Format("{0} ({1}),", aConflictingAttribute.Key, aConflictingAttribute.Value));
            }

            sb.Remove(sb.Length - 1, 1);

            _msg = String.Format("It is not possible to drop {0} of vertex type {1} because there are remaining references from the following attributes: {2}" + Environment.NewLine + "Please remove them in previous.", VertexAttributeName, VertexTypeName, sb.ToString());
           
        }

        #endregion

    }
}
