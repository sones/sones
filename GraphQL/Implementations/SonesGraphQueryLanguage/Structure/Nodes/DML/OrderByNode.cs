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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class OrderByNode : AStructureNode, IAstNodeInit
    {
        public OrderByDefinition OrderByDefinition { get; private set; }

        private SortDirection _OrderDirection;
        private List<OrderByAttributeDefinition> _OrderByAttributeList;


        public OrderByNode() { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]) && parseNode.ChildNodes[3].ChildNodes[0].Term.Name.ToUpper() == "DESC")
                    _OrderDirection = SortDirection.Desc;
                else
                    _OrderDirection = SortDirection.Asc;

                _OrderByAttributeList = new List<OrderByAttributeDefinition>();

                foreach (ParseTreeNode treeNode in parseNode.ChildNodes[2].ChildNodes)
                {
                    if (treeNode.AstNode != null && treeNode.AstNode is IDNode)
                    {

                        _OrderByAttributeList.Add(new OrderByAttributeDefinition(((IDNode)treeNode.AstNode).IDChainDefinition, null));
                    }
                    else
                    {
                        _OrderByAttributeList.Add(new OrderByAttributeDefinition(null, treeNode.Token.ValueString));
                    }
                }

                OrderByDefinition = new OrderByDefinition(_OrderDirection, _OrderByAttributeList);
            }
        }

        #endregion
    }
}
