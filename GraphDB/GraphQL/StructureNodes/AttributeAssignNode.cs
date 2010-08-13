/* <id name="GraphDB – Attribute Assign astnode" />
 * <copyright file="AttributeAssignNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute assign statement.</summary>
 */

#region Usings

using System;
using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of attribute assign statement.
    /// </summary>
    public class AttributeAssignNode : AStructureNode, IAstNodeInit
    {

        #region Data

        public AAttributeAssignOrUpdate AttributeValue { get; private set; }

        private IDChainDefinition _AttributeIDNode = null;
        //private ParseTreeNode _Node;

        #endregion

        #region constructor

        public AttributeAssignNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region get myAttributeName

            _AttributeIDNode = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;

            var attributeType = GraphDBTypeMapper.ConvertGraph2CSharp(parseNode.ChildNodes[2].Term.GetType().Name);

            #endregion

            var _Node = parseNode.ChildNodes[2];

            if (_Node.Token != null)
            {
                AttributeValue = new AttributeAssignOrUpdateValue(_AttributeIDNode, attributeType, _Node.Token.Value);
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
                        throw new GraphDBException(new Error_InvalidTuple("Could not extract BinaryExpressionNode from TupleNode."));
                    }
                }
                else
                {
                    throw new GraphDBException(new Error_InvalidTuple("It is not possible to have more than one binary expression in one tuple. Please check brackets."));
                }

                #endregion
            }
            else if (_Node.AstNode is IDNode)
            {
                throw new GraphDBException(new Error_InvalidAttributeValue(_AttributeIDNode.ToString(), (_Node.AstNode as IDNode).ToString()));
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
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

        }

        private String GenerateHelperMessage()
        {
            return string.Format(Environment.NewLine + "Try to use REF/REFERENCE (edge) or SETOF/LISTOF (hyperedge).");
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
