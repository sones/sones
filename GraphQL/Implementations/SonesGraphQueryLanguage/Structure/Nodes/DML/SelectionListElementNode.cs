using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an ColumnItem node
    /// </summary>
    public sealed class SelectionListElementNode : AStructureNode, IAstNodeInit
    {
        #region data

        public TypesOfColumnSource TypeOfColumnSource { get; private set; }
        public AExpressionDefinition ColumnSourceValue { get; private set; }
        public String AliasId { get; private set; }
        public String TypeName { get; private set; }
        public TypesOfSelect SelType { get; private set; }
        public SelectValueAssignment ValueAssignment { get; private set; }

        #endregion

        #region constructor

        public SelectionListElementNode()
        {
            ColumnSourceValue = null;
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[0].Token != null)
            {
                if (parseNode.ChildNodes[0].Token.Text == SonesGQLConstants.ASTERISKSYMBOL)
                {
                    SelType = TypesOfSelect.Asterisk;
                }
                else
                {
                    throw new NotImplementedQLException(parseNode.ChildNodes[0].Token.Text);
                }
            }
            else if (parseNode.ChildNodes[0].AstNode is SelByTypeNode)
            {
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
                    if ((parseNode.ChildNodes[0].ChildNodes != null && parseNode.ChildNodes[0].ChildNodes.Count > 0) && parseNode.ChildNodes[0].ChildNodes[0].AstNode != null)
                    {
                        astNode = parseNode.ChildNodes[0].ChildNodes[0].AstNode;
                    }
                    else
                    {
                        if (parseNode.ChildNodes[0].ChildNodes != null && parseNode.ChildNodes[0].ChildNodes.Count > 0)
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

                    if (HasChildNodes(parseNode.ChildNodes[1]))// && parseNode.ChildNodes.Last().AstNode is AliasNode)
                    {
                        AliasId = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString; //(parseNode.ChildNodes.Last().AstNode as AliasNode).AliasId;
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

                    if (HasChildNodes(parseNode.ChildNodes[1]))//Last().AstNode is AliasNode)
                    {
                        AliasId = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;//(parseNode.ChildNodes.Last().AstNode as AliasNode).AliasId;
                    }
                    else
                    {
                        AliasId = aAggregateNode.FuncDefinition.SourceParsedString;
                    }

                    #endregion
                }

                else
                {
                    throw new NotImplementedQLException(parseNode.ChildNodes[0].AstNode.GetType().Name);
                }

                #endregion
            }
        }

        #endregion
    }
}
