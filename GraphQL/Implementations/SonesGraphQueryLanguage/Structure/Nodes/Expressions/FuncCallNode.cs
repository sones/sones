using System;
using System.Linq;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    public class FuncCallNode : AStructureNode
    {
        public ChainPartFuncDefinition FuncDefinition { get; private set; }
        protected String _FuncName = null;
        private String _SourceParsedString = String.Empty;

        #region constructor

        public FuncCallNode()
        {
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            System.Diagnostics.Debug.Assert(parseNode.Span.Location.Line == 0);

            _SourceParsedString = context.CurrentParseTree.SourceText.Substring(parseNode.Span.Location.Position, parseNode.Span.Length);

            _FuncName = parseNode.ChildNodes[0].Token.ValueString.ToUpper();

            FuncDefinition = new ChainPartFuncDefinition(); // new
            FuncDefinition.SourceParsedString = _SourceParsedString; // new
            FuncDefinition.FuncName = _FuncName; // new

            #region Retrieve the parameters

            if (parseNode.ChildNodes.Count > 1)// && parseNode.ChildNodes[1].HasChildNodes())
            {

                if (parseNode.ChildNodes[1].Token != null)
                {
                    if (parseNode.ChildNodes[1].Token.Terminal.Name == "*")
                    {
                        #region asterisk

                        if (GetTypeReferenceDefinitions(context).Count != 1)
                        {
                            throw new FunctionParameterInvalidReferenceException("It is not allowed to execute a function with an asterisk as parameter and more than one type.");
                        }

                        var listOfReferences = GetTypeReferenceDefinitions(context);
                        var idChainDef = new IDChainDefinition(listOfReferences.First().TypeName, listOfReferences);

                        FuncDefinition.Parameters.Add(idChainDef); // new

                        #endregion
                    }
                    else
                    {
                        GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
                    }
                }
                else
                {
                    GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
                }
            }
            else if (parseNode.ChildNodes.Count > 1)
            {
                GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
            }

            #endregion
        }

        #endregion

        #region private helper 

        private void GenerateData(ParsingContext context, List<ParseTreeNode> parseNodes)
        {

            Int32 paramNum = 0;

            foreach (ParseTreeNode node in parseNodes)
            {
                if (node.AstNode == null && HasChildNodes(node))
                {
                    foreach (var aExpressionNode in node.ChildNodes)
                    {
                        if (aExpressionNode.AstNode is IDNode)
                        {
                            FuncDefinition.Parameters.Add((aExpressionNode.AstNode as IDNode).IDChainDefinition); // new
                        }

                        else if (aExpressionNode.AstNode is BinaryExpressionNode)
                        {
                            FuncDefinition.Parameters.Add((aExpressionNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition); // new
                        }

                        else
                        {
                            FuncDefinition.Parameters.Add(new ValueDefinition(aExpressionNode.Token.Value)); // new
                        }

                        paramNum++;
                    }
                }

            }

        }


        #endregion
    }
}
