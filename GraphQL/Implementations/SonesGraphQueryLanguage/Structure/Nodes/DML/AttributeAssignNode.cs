using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of attribute assign statement.
    /// </summary>
    public sealed class AttributeAssignNode : AStructureNode, IAstNodeInit
    {

        #region Data

        public AAttributeAssignOrUpdate AttributeValue { get; private set; }

        private IDChainDefinition _AttributeIDNode = null;

        #endregion

        #region constructor

        public AttributeAssignNode()
        {

        }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get myAttributeName

            _AttributeIDNode = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;

            #endregion

            var _Node = parseNode.ChildNodes[2];

            if (_Node.Token != null)
            {
                AttributeValue = new AttributeAssignOrUpdateValue(_AttributeIDNode, _Node.Token.Value);
            }
            else if (_Node.AstNode is BinaryExpressionNode)
            {
                #region binary expression

                AttributeValue = new AttributeAssignOrUpdateExpression(_AttributeIDNode, (_Node.AstNode as BinaryExpressionNode).BinaryExpressionDefinition);

                #endregion
            }
            else if (_Node.AstNode is TupleNode)
            {
                #region Tuple

                TupleNode tempTupleNode = (TupleNode)_Node.AstNode;

                if (tempTupleNode.TupleDefinition.Count() == 1)
                {
                    if (tempTupleNode.TupleDefinition.First().Value is BinaryExpressionDefinition)
                    {
                        AttributeValue = new AttributeAssignOrUpdateExpression(_AttributeIDNode, tempTupleNode.TupleDefinition.First().Value as BinaryExpressionDefinition);
                    }
                    else
                    {
                        throw new InvalidTupleException("Could not extract BinaryExpressionNode from TupleNode.");
                    }
                }
                else
                {
                    throw new InvalidTupleException("It is not possible to have more than one binary expression in one tuple. Please check brackets.");
                }

                #endregion
            }
            else if (_Node.AstNode is IDNode)
            {
                throw new InvalidVertexAttributeValueException(_AttributeIDNode.ToString(), (_Node.AstNode as IDNode).ToString());
            }
            else if (_Node.AstNode is SetRefNode)
            {
                #region setref

                AttributeValue = new AttributeAssignOrUpdateSetRef(_AttributeIDNode, (_Node.AstNode as SetRefNode).SetRefDefinition);

                #endregion
            }
            else if ((_Node.AstNode is CollectionOfDBObjectsNode))
            {
                #region collection like list

                AttributeValue = new AttributeAssignOrUpdateList((_Node.AstNode as CollectionOfDBObjectsNode).CollectionDefinition, _AttributeIDNode, true);

                #endregion
            }
            else
            {
                throw new NotImplementedQLException ("");
            }
        }

        #endregion
    }
}
