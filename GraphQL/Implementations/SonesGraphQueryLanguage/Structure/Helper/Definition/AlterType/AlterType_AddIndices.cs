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
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Adds indices to a vertex
    /// </summary>
    public sealed class AlterType_AddIndices : AAlterTypeCommand
    {
        public readonly List<IndexDefinition> IdxDefinitionList;

        public AlterType_AddIndices(List<IndexDefinition> listOfIndices)
        {
            IdxDefinitionList = listOfIndices;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.AddIndex; }
        }

        /// <summary>
        /// Checks whether the index can be changed
        /// </summary>
        /// <param name="idxDef">List of index attribute definitions</param>
        /// <param name="type">The db type that is to be altered</param>
        /// <returns>An exceptional</returns>
        private void CheckIndexTypeReference(List<IndexAttributeDefinition> idxDef, IVertexType type)
        {
            foreach (var idx in idxDef)
            {
                if (idx.IndexType != null && idx.IndexType != type.Name)
                {
                    throw new CouldNotAlterIndexOnTypeException(idx.IndexType);
                }
            }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
