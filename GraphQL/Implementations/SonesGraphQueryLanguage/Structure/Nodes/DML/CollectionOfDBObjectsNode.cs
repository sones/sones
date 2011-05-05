using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).
    /// </summary>
    public sealed class CollectionOfDBObjectsNode : AStructureNode, IAstNodeInit
    {
        public CollectionDefinition CollectionDefinition { get; private set; }

        #region constructor

        public CollectionOfDBObjectsNode()
        {
            
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var multiplyString = parseNode.ChildNodes[0].Token.Text.ToUpper();
            CollectionType collectionType;

            switch (multiplyString)
            {
                case SonesGQLConstants.LISTOF:

                    collectionType = CollectionType.List;

                    break;

                case SonesGQLConstants.SETOF:

                    collectionType = CollectionType.Set;

                    break;

                case SonesGQLConstants.SETOFUUIDS:

                    collectionType = CollectionType.SetOfUUIDs;

                    break;
                default:

                    throw new NotImplementedException("");
            }

            if (parseNode.ChildNodes[1].AstNode is TupleNode)
            {
                CollectionDefinition = new CollectionDefinition(collectionType, ((TupleNode)parseNode.ChildNodes[1].AstNode).TupleDefinition);
            }
            else
            {
                CollectionDefinition = new CollectionDefinition(collectionType, (VertexTypeVertexIDCollectionNode)parseNode.ChildNodes[1].AstNode);
            }
        }

        #endregion
    }
}
