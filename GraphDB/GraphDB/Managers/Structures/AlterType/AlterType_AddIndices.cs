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
 * AlterType_AddIndices
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Managers.Structures;

using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_AddIndices : AAlterTypeCommand
    {

        private List<IndexDefinition> _IdxDefinitionList;

        public AlterType_AddIndices(List<IndexDefinition> listOfIndices)
        {
            _IdxDefinitionList = listOfIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        private Exceptional CheckIndexTypeReference(List<IndexAttributeDefinition> idxDef, GraphDBType type)
        {
            foreach (var idx in idxDef)
            {
                if (idx.IndexType != null && idx.IndexType != type.Name)
                {
                    return new Exceptional(new Error_CouldNotAlterIndexOnType(idx.IndexType));
                }
            }

            return Exceptional.OK;
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {

            var retExceptional = new Exceptional();

            foreach (var idxDef in _IdxDefinitionList)
            {

                var checkIdx = CheckIndexTypeReference(idxDef.IndexAttributeDefinitions, graphDBType);

                if (!checkIdx.Success())
                {
                    retExceptional.PushIExceptional(checkIdx);
                }
                else
                {
                    var result = dbContext.DBIndexManager.CreateIndex(dbContext, graphDBType.Name, idxDef.IndexName, idxDef.Edition, idxDef.IndexType, idxDef.IndexAttributeDefinitions);

                    if (!result.Success())
                    {
                        retExceptional.PushIExceptional(result);
                    }
                }

            }

            return retExceptional;

        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            var payload         = new Dictionary<String, Object>();
            var indicesReadouts = new List<Vertex>();

            payload.Add("TYPE", myGraphDBType.Name);

            if (_IdxDefinitionList.IsNotNullOrEmpty())
            {

                payload.Add("ACTION", "ADD INDICES");

                foreach (var idxDef in _IdxDefinitionList)
                {

                    var payloadPerIndex = new Dictionary<String, Object>();

                    payloadPerIndex.Add("NAME",      idxDef.IndexName);
                    payloadPerIndex.Add("EDITION",   idxDef.Edition);
                    payloadPerIndex.Add("INDEXTYPE", idxDef.IndexType);

                    if (idxDef.IndexAttributeDefinitions.Count == 1)
                    {
                        payloadPerIndex.Add("ATTRIBUTE", idxDef.IndexAttributeDefinitions[0].IndexAttribute);
                    }
                    else
                    {
                        String attributes = String.Empty;

                        idxDef.IndexAttributeDefinitions.ForEach(item => attributes += item.IndexAttribute + " ");
                        payloadPerIndex.Add("ATTRIBUTES", attributes);
                    }

                    indicesReadouts.Add(new Vertex(payloadPerIndex));

                }

                payload.Add("INDICES", indicesReadouts);

            }

            return new List<Vertex> { new Vertex(payload) };

        }

    }



}
