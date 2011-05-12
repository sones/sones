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
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Helper.Enums;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Removal of attribute indices
    /// </summary>
    public sealed class AlterType_DropIndices : AAlterTypeCommand
    {
        /// <summary>
        /// The list of the indices
        /// </summary>
        public readonly Dictionary<String, String> IdxDropList;

        public AlterType_DropIndices(Dictionary<String, String> myIndices)
        {
            IdxDropList = myIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropIndex; }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
