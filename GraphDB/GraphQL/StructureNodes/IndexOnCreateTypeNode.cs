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


#region Usings

using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Errors: Error_ArgumentException
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    public class IndexOnCreateTypeNode : AStructureNode
    {

        #region Properties

        public List<IndexDefinition> ListOfIndexDefinitions { get; private set; }

        #endregion

        #region constructors

        public IndexOnCreateTypeNode()
        { }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            ListOfIndexDefinitions = new List<IndexDefinition>();

            if (parseNode.ChildNodes[1].AstNode is IndexOptOnCreateTypeMemberNode)
            {
                var aIDX = (IndexOptOnCreateTypeMemberNode)parseNode.ChildNodes[1].AstNode;
                ParsingResult.PushIExceptional(aIDX.ParsingResult);

                ListOfIndexDefinitions.Add(aIDX.IndexDefinition);
            }

            else
            {
                var idcs = parseNode.ChildNodes[1].ChildNodes.Select(child =>
                {
                    ParsingResult.PushIExceptional(((IndexOptOnCreateTypeMemberNode)child.AstNode).ParsingResult);
                    return ((IndexOptOnCreateTypeMemberNode)child.AstNode).IndexDefinition;
                });
                ListOfIndexDefinitions.AddRange(idcs);
            }

        }


    }

}
