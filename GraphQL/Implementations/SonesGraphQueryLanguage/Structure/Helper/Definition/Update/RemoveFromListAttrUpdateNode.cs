using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Nodes;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// This node is requested in case of an RemoveFromListAttrUpdate node.
    /// </summary>
    public class RemoveFromListAttrUpdateNode: AStructureNode, IAstNodeInit
    {
        public AttributeRemoveList AttributeRemoveList { get; protected set; }

        #region constructor

        public RemoveFromListAttrUpdateNode()
        {

        }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as RemoveFromListAttrUpdateNode;

            AttributeRemoveList = content.AttributeRemoveList;
        }

        #endregion
    }
}
