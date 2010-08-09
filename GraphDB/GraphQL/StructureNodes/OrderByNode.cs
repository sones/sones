
#region Usings

using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.DataStructures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class OrderByNode : AStructureNode, IAstNodeInit
    {

        public OrderByDefinition OrderByDefinition { get; private set; }

        private SortDirection _OrderDirection;
        private List<OrderByAttributeDefinition> _OrderByAttributeList;
       

        public OrderByNode() { }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes() && parseNode.ChildNodes[3].ChildNodes[0].Term.Name.ToUpper() == "DESC")
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

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
