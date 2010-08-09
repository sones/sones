/* <id name="PandoraDB – WhereExpressionNode" />
 * <copyright file="WhereExpressionNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Daniel Kirstenpfad
 * <summary>This node is requested in case of where clause.</summary>
 */

#region Usings

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class WhereExpressionNode : AStructureNode
    {

        private BinaryExpressionNode _binExprNode = null;

        public WhereExpressionNode()
        { }

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            if (myParseTreeNode.HasChildNodes())
            {
                _binExprNode = (BinaryExpressionNode) myParseTreeNode.ChildNodes[1].AstNode;
            }
        }

        #endregion

        public override string ToString()
        {
            return "whereClauseOpt";
        }

        public BinaryExpressionNode BinExprNode { get { return _binExprNode; } }

    }

}
