
#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Operators;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class UnaryExpressionNode : AStructureNode
    {

        #region Data

        private String _OperatorSymbol;
        private Object _Term;

        #endregion

        public UnaryExpressionDefinition UnaryExpressionDefinition { get; private set; }

        #region Accessor

        public String OperatorSymbol { get; private set; } 
        public AExpressionDefinition Expression { get; private set; } 

        #endregion

        #region Constructor

        public UnaryExpressionNode()
        { }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
            {

                if (myParseTreeNode.ChildNodes[1].AstNode == null)
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                _OperatorSymbol = myParseTreeNode.ChildNodes[0].Token.Text;
                Expression = GetExpressionDefinition(myParseTreeNode.ChildNodes[1]);

            }

            System.Diagnostics.Debug.Assert(Expression != null);

            UnaryExpressionDefinition = new UnaryExpressionDefinition(_OperatorSymbol, Expression);

        }

        #endregion

    }

}
