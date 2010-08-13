/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – DBObject Readout" />
 * <copyright file="DBObjectReadout.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Carries information of DBObjects but without their whole functionality.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.Structures.Result
{

    /// <summary>
    /// Carries information of DBObjects but without their whole functionality.
    /// </summary>
    public class DBObjectReadoutGroup : DBObjectReadout
    {

        public IEnumerable<DBObjectReadout> GrouppedVertices { get; protected set; }

        public DBObjectReadoutGroup(IDictionary<String, Object> myAttributes, IEnumerable<DBObjectReadout> myGrouppedVertices)
            : base (myAttributes)
        {
            GrouppedVertices = myGrouppedVertices;
        }

    }

}
