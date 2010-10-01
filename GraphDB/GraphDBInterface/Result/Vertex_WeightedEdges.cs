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

/* <id name="GraphDB – WeightedAttributeObject" />
 * <copyright file="WeightedAttributeObject.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Carries information of DBObject WeightedList Attribute.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Result
{

    /// <summary>
    /// Carries information of DBObjects but without their whole functionality.
    /// </summary>
    public class Vertex_WeightedEdges : Vertex
    {

        public Object Weight   { get; private set; }
        public String TypeName { get; private set; }

        public Vertex_WeightedEdges(IDictionary<String, Object> myAttributes, object myWeight, String myTypeName)
            : base (myAttributes)
        {
            Weight   = myWeight;
            TypeName = myTypeName;
        }

    }

}
