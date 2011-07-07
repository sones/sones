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
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Node to get index description of a given type
    /// </summary>
    public sealed class DescribeIndexNode : ADescrNode, IAstNodeInit
    {
        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeIndexDefinition; }
        }
        private DescribeIndexDefinition _DescribeIndexDefinition;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            String vertexType = ((ATypeNode)parseNode.ChildNodes[1].AstNode).ReferenceAndType.TypeName;
            String indexName = parseNode.ChildNodes[2].ChildNodes.Count > 0 ? parseNode.ChildNodes[2].ChildNodes[0].Token.ValueString : String.Empty;
            String edition = String.Empty;

            if (parseNode.ChildNodes[3].ChildNodes.Count > 0)
            {
                edition = parseNode.ChildNodes[3].ChildNodes[1].Token.ValueString;
            }

            _DescribeIndexDefinition = new DescribeIndexDefinition(vertexType, indexName, edition);
        }

        #endregion
    }
}
