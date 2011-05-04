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
    public sealed class RemoveFromListAttrUpdateScopeNode : RemoveFromListAttrUpdateNode
    {
        #region propertys

        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region constructor

        public RemoveFromListAttrUpdateScopeNode()
        { }

        #endregion

        /// <summary>
        /// Get the scope of an remove list update
        /// </summary>
        /// <param name="context">Irony compiler context</param>
        /// <param name="parseNode">The parse node that contains the information</param>
        public void DirectInit(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[1].AstNode is CollectionOfDBObjectsNode)
            {
                var collection = parseNode.ChildNodes[1].AstNode as CollectionOfDBObjectsNode;

                if (collection.CollectionDefinition.CollectionType != CollectionType.SetOfUUIDs)
                {
                    throw new NotImplementedQLException("");
                }

                TupleDefinition = collection.CollectionDefinition.TupleDefinition;
            }

            if (parseNode.ChildNodes[1].AstNode is TupleNode)
            {
                TupleDefinition = ((TupleNode)parseNode.ChildNodes[1].AstNode).TupleDefinition;
            }
        }
        
    }
}
