#region Usings

using System.Collections.Generic;
using sones.GraphQL.StructureNodes;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Helper.Misc;

#endregion

namespace sones.GraphQL.StructureNodes
{

    /// <summary>
    /// Abstract class for all structure nodes.
    /// </summary>
    public abstract class AStructureNode
    {
        #region protected helper methods

        /// <summary>
        /// Extracts a AExpressionDefinition from the ParseTreeNode
        /// </summary>
        /// <param name="myParseTreeNode"></param>
        /// <returns></returns>
        protected AExpressionDefinition GetExpressionDefinition(ParseTreeNode myParseTreeNode)
        {
            AExpressionDefinition retVal = null;
            if (myParseTreeNode.Term is NonTerminal)
            {
                #region left is NonTerminal

                if (myParseTreeNode.AstNode is IDNode)
                {
                    retVal = (myParseTreeNode.AstNode as IDNode).IDChainDefinition;
                }
                else if (myParseTreeNode.AstNode is TupleNode)
                {
                    retVal = (myParseTreeNode.AstNode as TupleNode).TupleDefinition;
                }
                else if (myParseTreeNode.AstNode is BinaryExpressionNode)
                {
                    retVal = (myParseTreeNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition;
                }
                else if (myParseTreeNode.AstNode is UnaryExpressionNode)
                {
                    retVal = (myParseTreeNode.AstNode as UnaryExpressionNode).UnaryExpressionDefinition;
                }
                else if (myParseTreeNode.AstNode is AggregateNode)
                {
                    retVal = (myParseTreeNode.AstNode as AggregateNode).AggregateDefinition;
                }

                #endregion
            }
            else
            {
                #region No NonTerminal

                retVal = new ValueDefinition(myParseTreeNode.Token.Value);

                #endregion
            }

            return retVal;
        }

        #endregion
    }

}
