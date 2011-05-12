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
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexOnCreateTypeNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public List<IndexDefinition> ListOfIndexDefinitions { get; private set; }

        #endregion

        #region constructors

        public IndexOnCreateTypeNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ListOfIndexDefinitions = new List<IndexDefinition>();

            if (parseNode.ChildNodes[1].AstNode is IndexOptOnCreateTypeMemberNode)
            {
                var aIDX = (IndexOptOnCreateTypeMemberNode)parseNode.ChildNodes[1].AstNode;

                ListOfIndexDefinitions.Add(aIDX.IndexDefinition);
            }

            else
            {
                var idcs = parseNode.ChildNodes[1].ChildNodes.Select(child =>
                {
                    return ((IndexOptOnCreateTypeMemberNode)child.AstNode).IndexDefinition;
                });
                ListOfIndexDefinitions.AddRange(idcs);
            }
        }

        #endregion
    }
}
