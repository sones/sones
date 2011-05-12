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
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public sealed class AttributeDefinitionNode : AStructureNode, IAstNodeInit
    {

        #region constructor

        public AttributeDefinitionNode()
        {

        }

        #endregion

        public AttributeDefinition AttributeDefinition { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            AttributeDefinition = new AttributeDefinition(((VertexTypeNode)parseNode.ChildNodes[0].AstNode).DBTypeDefinition, parseNode.ChildNodes[1].Token.ValueString, ((AttrDefaultValueNode)(parseNode.ChildNodes[2].AstNode)).Value);
        }

        #endregion
    }
}
