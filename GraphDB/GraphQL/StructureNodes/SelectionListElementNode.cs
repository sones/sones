/* <id name="GraphDB – ColumnItem node" />
 * <copyright file="ColumnItemNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of ColumnItemNode.</summary>
 */

#region Usings

using System;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.GraphQL.Structure;
using sones.GraphDB.Managers.Select;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{


    /// <summary>
    /// This node is requested in case of an ColumnItem node
    /// </summary>
    public class SelectionListElementNode : AStructureNode, IAstNodeInit
    {


        #region Accessors

        public TypesOfColumnSource TypeOfColumnSource   { get; private set; } 
        public AExpressionDefinition ColumnSourceValue  { get; private set; }
        public String AliasId                           { get; private set; }
        public String TypeName                          { get; private set; }
        public TypesOfSelect SelType                    { get; private set; }
        public SelectValueAssignment ValueAssignment    { get; private set; }

        #endregion

        #region constructor

        public SelectionListElementNode()
        {
            ColumnSourceValue = null;
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[0].Token != null)
            {
                if (parseNode.ChildNodes[0].Token.Text == DBConstants.ASTERISKSYMBOL)
                {
                    SelType = TypesOfSelect.Asterisk;
                }
                else if (parseNode.ChildNodes[0].Token.Text == DBConstants.RHOMBSYMBOL)
                {
                    SelType = TypesOfSelect.Rhomb;
                }
                else if (parseNode.ChildNodes[0].Token.Text == DBConstants.MINUSSYMBOL)
                {
                    SelType = TypesOfSelect.Minus;
                }
                else if (parseNode.ChildNodes[0].Token.Text == DBConstants.Comperator_Greater)
                {
                    SelType = TypesOfSelect.Gt;
                }
                else if (parseNode.ChildNodes[0].Token.Text == DBConstants.Comperator_Smaller)
                {
                    SelType = TypesOfSelect.Lt;
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), parseNode.ChildNodes[0].Token.Text));
                }
            }
            else if (parseNode.ChildNodes[0].AstNode is SelByTypeNode)
            {
                SelType = TypesOfSelect.Ad;
                TypeName = ((SelByTypeNode)parseNode.ChildNodes[0].AstNode).TypeName;
            }
            else
            {

                #region IDNode or AggregateNode

                SelType = TypesOfSelect.None;
                object astNode = parseNode.ChildNodes[0].AstNode;

                if (astNode == null)
                {
                    // why the hack are functions without a caller in the child/child?
                    if (parseNode.ChildNodes[0].HasChildNodes() && parseNode.ChildNodes[0].ChildNodes[0].AstNode != null)
                    {
                        astNode = parseNode.ChildNodes[0].ChildNodes[0].AstNode;
                    }
                    else
                    {
                        if (parseNode.ChildNodes[0].HasChildNodes())
                        {
                            throw new ArgumentException("This is not a valid IDNode: " + (parseNode.ChildNodes[0].ChildNodes.Aggregate("", (result, elem) => { result += elem.Token.Text + "."; return result; })));
                        }
                        else
                        {
                            throw new ArgumentException("Found an invalid IDNode");
                        }
                    }
                }


                if (astNode is IDNode)
                {
                    IDNode aIDNode = astNode as IDNode;

                    #region ID

                    TypeOfColumnSource = TypesOfColumnSource.ID;

                    #region get value

                    ColumnSourceValue = ((IDNode)aIDNode).IDChainDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count > 2)// && parseNode.ChildNodes.Last().AstNode is AliasNode)
                    {
                        AliasId = parseNode.ChildNodes[3].Token.ValueString; //(parseNode.ChildNodes.Last().AstNode as AliasNode).AliasId;
                    }

                    #endregion

                    #endregion
                }

                else if (astNode is AggregateNode)
                {
                    #region aggregate

                    TypeOfColumnSource = TypesOfColumnSource.Aggregate;

                    AggregateNode aAggregateNode = (AggregateNode)astNode;

                    ColumnSourceValue = aAggregateNode.AggregateDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count > 1)//Last().AstNode is AliasNode)
                    {
                        AliasId = parseNode.ChildNodes[2].Token.ValueString;//(parseNode.ChildNodes.Last().AstNode as AliasNode).AliasId;
                    }
                    else
                    {
                        AliasId = aAggregateNode.AggregateDefinition.ChainPartAggregateDefinition.SourceParsedString;
                    }

                    #endregion
                }

                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), parseNode.ChildNodes[0].AstNode.GetType().Name));
                }

                #endregion

                if (parseNode.ChildNodes.Count > 1)
                {

                    #region SelectValueAssignmentNode

                    if (parseNode.ChildNodes[1].AstNode is SelectValueAssignmentNode)
                    {
                        ValueAssignment = (parseNode.ChildNodes[1].AstNode as SelectValueAssignmentNode).ValueAssignment;
                        //ValueAssignment = new Tuple<ValueAssignmentType, object>(ValueAssignmentType.Always, parseNode.ChildNodes[2].Token.Value);
                    }

                    #endregion

                }

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
