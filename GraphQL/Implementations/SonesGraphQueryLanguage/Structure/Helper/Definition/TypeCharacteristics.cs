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

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// This class holds all characteristics of an specific attribute of a DB type.
    /// </summary>
    public sealed class TypeCharacteristics
    {

        /// <summary>
        /// This attribute is an incoming edge
        /// </summary>
        public Boolean IsIncomingEdge { get; set; }

        /// <summary>
        /// If set to true, all object of the RelatedGraphType with this attribute a unique for this RelatedGraphType.
        /// If more attribute are set to UNIQUE, all together are unique.
        /// </summary>
        public Boolean IsUnique { get; set; }

        /// <summary>
        /// The Mandatory flag
        /// </summary>
        public Boolean IsMandatory { get; set; }

        /// <summary>
        /// Create standard Unique typeCharacteristic (no Queue, no Weighted)
        /// </summary>
        public TypeCharacteristics()
        {
            IsIncomingEdge = false;
            IsUnique = false;
            IsMandatory = false;
        }
    }
}
