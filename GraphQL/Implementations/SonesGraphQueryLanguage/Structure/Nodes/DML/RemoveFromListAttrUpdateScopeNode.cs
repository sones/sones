using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// contains the scope of a list update statement
    /// </summary>
    public sealed class RemoveFromListAttrUpdateScopeNode : AStructureNode, IAstNodeInit
    {
        #region propertys

        public Object TupleDefinition { get; private set; }

        #endregion

        #region constructor

        public RemoveFromListAttrUpdateScopeNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[1].AstNode is CollectionOfDBObjectsNode)
            {
                var collection = parseNode.ChildNodes[1].AstNode as CollectionOfDBObjectsNode;

                TupleDefinition = collection.CollectionDefinition.TupleDefinition;
            }
            else if (parseNode.ChildNodes[1].AstNode is TupleNode)
            {
                TupleDefinition = ((TupleNode)parseNode.ChildNodes[1].AstNode).TupleDefinition;
            }
        }

        #endregion
    }
}
