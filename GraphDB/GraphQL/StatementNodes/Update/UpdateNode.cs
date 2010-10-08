/* 
 * GQL - UpdateNode
 * (c) Henning Rauch, Stefan Licht, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Update
{

    /// <summary>
    /// This node is requested in case of an Update statement.
    /// </summary>
    class UpdateNode : AStatement
    {

        #region Data

        private HashSet<AAttributeAssignOrUpdateOrRemove> _listOfUpdates;

        private BinaryExpressionDefinition _WhereExpression;
        private String _TypeName;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Update"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public UpdateNode()
        {
        }

        #endregion       

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var qresult = myIGraphDBSession.Update(_TypeName, _listOfUpdates, _WhereExpression);
            qresult.PushIExceptional(ParsingResult);
            return qresult;

        }

        /// <summary>
        /// Gets the content of an UpdateStatement.
        /// </summary>
        /// <param name="context">CompilerContext of Irony.</param>
        /// <param name="parseNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the GraphDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            #region get Type

            _TypeName = (myParseTreeNode.ChildNodes[1].AstNode as ATypeNode).ReferenceAndType.TypeName;

            #endregion

            #region get myAttributes

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {
                var AttrUpdateOrAssign = (AttrUpdateOrAssignListNode)myParseTreeNode.ChildNodes[3].AstNode;
                _listOfUpdates = AttrUpdateOrAssign.ListOfUpdate;
                base.ParsingResult.PushIExceptional(AttrUpdateOrAssign.ParsingResult);
            }

            #endregion

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                var tempWhereNode = (WhereExpressionNode) myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;

            }

            #endregion

        }
    } 
}
