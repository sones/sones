using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class AttrAssignListNode : AStructureNode, IAstNodeInit
    {
        public List<AAttributeAssignOrUpdate> AttributeAssigns { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            AttributeAssigns = new List<AAttributeAssignOrUpdate>();
            foreach (ParseTreeNode aAttributeAssignment in parseNode.ChildNodes)
            {
                AttributeAssigns.Add((aAttributeAssignment.AstNode as AttributeAssignNode).AttributeValue);
            }

        }

        #endregion
    }
}
