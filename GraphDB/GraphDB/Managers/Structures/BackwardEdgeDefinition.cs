/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * BackwardEdgeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.EdgeTypes;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class BackwardEdgeDefinition
    {

        public BackwardEdgeDefinition(string myAttributeName, string myTypeName, string myTypeAttributeName, IEdgeType myEdgeType)
        {
            AttributeName = myAttributeName;
            TypeName = myTypeName;
            TypeAttributeName = myTypeAttributeName;
            EdgeType = myEdgeType;
        }

        #region Data

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        public String TypeName { get; private set; }

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        public String TypeAttributeName { get; private set; }

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        public String AttributeName { get; private set; }

        /// <summary>
        /// The Type of the edge, currently EdgeTypeList or EdgeTypeWeightedList
        /// </summary>
        public IEdgeType EdgeType { get; private set; }

        #endregion
    
    }
}
