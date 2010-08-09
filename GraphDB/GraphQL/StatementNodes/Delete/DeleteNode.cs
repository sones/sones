using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.GraphQL.StatementNodes.Drop
{
    public class DeleteNode : AStatement
    {

        #region Data

        private BinaryExpressionDefinition _WhereExpression;
        
        private List<IDChainDefinition> _IDChainDefinitions;

        private List<TypeReferenceDefinition> _TypeReferenceDefinitions;
        

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Delete"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _IDChainDefinitions = new List<IDChainDefinition>();

            _TypeReferenceDefinitions = (myParseTreeNode.ChildNodes[1].AstNode as TypeListNode).Types;

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {
                IDNode tempIDNode;
                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode is IDNode)
                    {
                        tempIDNode = (IDNode)_ParseTreeNode.AstNode;
                        _IDChainDefinitions.Add(tempIDNode.IDChainDefinition);
                    }
                }
            }
            else
            {
                foreach (var type in _TypeReferenceDefinitions)
                {
                    var def = new IDChainDefinition();
                    def.AddPart(new ChainPartTypeOrAttributeDefinition(type.Reference));
                    _IDChainDefinitions.Add(def);
                }
            }

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinExprNode.BinaryExpressionDefinition;

            }

            #endregion
        
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            return graphDBSession.Delete(_TypeReferenceDefinitions, _IDChainDefinitions, _WhereExpression);

        }

    }
}
