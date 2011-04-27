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
