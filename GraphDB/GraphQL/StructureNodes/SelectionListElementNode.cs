/* <id name="PandoraDB – ColumnItem node" />
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

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an ColumnItem node
    /// </summary>
    public class SelectionListElementNode : AStructureNode, IAstNodeInit
    {

        public TypesOfSelect SelType { get; private set; }        
        
        #region Data

        TypesOfColumnSource         _TypeOfColumnSource;
        AExpressionDefinition       _ColumnSourceValue  = null;
        String                      _AliasId            = null;

        #endregion

        #region Accessors

        public TypesOfColumnSource TypeOfColumnSource { get { return _TypeOfColumnSource; } }
        public AExpressionDefinition ColumnSourceValue { get { return _ColumnSourceValue; } }
        public String AliasId { get { return _AliasId; } }

        #endregion

        #region constructor

        public SelectionListElementNode()
        {
            
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
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), parseNode.ChildNodes[0].Token.Text));
                }
            }

            else
            {
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

                    _TypeOfColumnSource = TypesOfColumnSource.ID;

                    #region get value

                    _ColumnSourceValue = ((IDNode)aIDNode).IDChainDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count > 1)
                    {
                        _AliasId = parseNode.ChildNodes[2].Token.ValueString;
                    }

                    #endregion

                    #endregion
                }

                else if (astNode is AggregateNode)
                {
                    #region aggregate

                    _TypeOfColumnSource = TypesOfColumnSource.Aggregate;

                    AggregateNode aAggregateNode = (AggregateNode)astNode;

                    _ColumnSourceValue = aAggregateNode.AggregateDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count.CompareTo(1) > 0)
                    {
                        _AliasId = parseNode.ChildNodes[2].Token.ValueString;
                    }
                    else
                    {
                        _AliasId = aAggregateNode.AggregateDefinition.ChainPartAggregateDefinition.SourceParsedString;
                    }

                    #endregion
                }

                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), parseNode.ChildNodes[0].AstNode.GetType().Name));
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
