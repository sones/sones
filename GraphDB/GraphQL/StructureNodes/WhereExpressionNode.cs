/* <id name="GraphDB – WhereExpressionNode" />
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
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class WhereExpressionNode : AStructureNode
    {

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        public WhereExpressionNode()
        { }

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            if (myParseTreeNode.HasChildNodes())
            {

                if (myParseTreeNode.ChildNodes[1].AstNode is TupleNode && (myParseTreeNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.TupleElements.Count == 1)
                {
                    var tuple = (myParseTreeNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.Simplyfy();
                    BinaryExpressionDefinition = (tuple.TupleElements[0].Value as BinaryExpressionDefinition);
                }
                else if (myParseTreeNode.ChildNodes[1].AstNode is BinaryExpressionNode)
                {
                    BinaryExpressionDefinition = ((BinaryExpressionNode)myParseTreeNode.ChildNodes[1].AstNode).BinaryExpressionDefinition;
                }
                //else
                //{
                //    throw new GraphDBException(new Errors.Error_GqlSyntax("Invalid tuple for where expression"));
                //}
            }
        }

        #endregion

        public override string ToString()
        {
            return "whereClauseOpt";
        }


    }

}
