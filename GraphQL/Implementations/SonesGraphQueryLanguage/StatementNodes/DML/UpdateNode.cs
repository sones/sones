using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class UpdateNode : AStatement, IAstNodeInit
    {
        #region Data

        /// <summary>
        /// The attributes to update / add / remove
        /// </summary>
        private HashSet<AAttributeAssignOrUpdateOrRemove> _listOfUpdates;

        /// <summary>
        /// Where Expression
        /// </summary>
        private BinaryExpressionDefinition _WhereExpression;

        /// <summary>
        /// The Name of the type which should be updated
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// The executed query
        /// </summary>
        private String Query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Type

            _TypeName = (parseNode.ChildNodes[1].AstNode as ATypeNode).ReferenceAndType.TypeName;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                var AttrUpdateOrAssign = (AttributeUpdateOrAssignListNode)parseNode.ChildNodes[3].AstNode;
                _listOfUpdates = AttrUpdateOrAssign.ListOfUpdate;
            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count != 0)
            {
                var tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Update"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        /// <summary>
        /// Executes the statement and returns a QueryResult.
        /// </summary>
        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            Query = myQuery;

            return myGraphDB.Update(mySecurityToken, myTransactionToken, new RequestUpdate(_TypeName), GenerateOutput);
        }

        #endregion

        #region helper

        private QueryResult GenerateOutput(IRequestStatistics myStats, IVertexType myVertexType)
        {
            return new QueryResult(Query, 
                                    "GQL", 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful, 
                                    new List<IVertexView> { new VertexView(new Dictionary<String, object> { { "UpdatedVertex", myVertexType } }, new Dictionary<String, IEdgeView>()) });
        }

        #endregion
    }
}
