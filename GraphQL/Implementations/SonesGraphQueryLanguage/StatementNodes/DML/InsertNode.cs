using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.DML;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertNode : AStatement, IAstNodeInit
    {
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private String _queryString;

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get type for name

            _TypeName = GetTypeReferenceDefinitions(context).First().TypeName;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                _AttributeAssignList = ((parseNode.ChildNodes[3].ChildNodes[1].AstNode as AttrAssignListNode).AttributeAssigns);
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Insert"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _queryString = myQuery;

            QueryResult result;

            try
            {
                result = myGraphDB.Insert<QueryResult>(
                        mySecurityToken,
                        myTransactionToken,
                        CreateRequest(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken),
                        CreateQueryResult);
            }
            catch (ASonesException e)
            {
                result = new QueryResult(_queryString, SonesGQLConstants.GQL, 0, ResultType.Failed, null, e);
            }

            return result;
        }

        #endregion

        #region private helper

        /// <summary>
        /// Creates the query result
        /// </summary>
        /// <param name="myStats">The stats of the request</param>
        /// <param name="myCreatedVertex">The vertex that has been created</param>
        /// <returns>The created query result</returns>
        private QueryResult CreateQueryResult(IRequestStatistics myStats, IVertex myCreatedVertex)
        {
            return null;
        }

        /// <summary>
        /// Creates the request for the graphdb
        /// </summary>
        /// <returns>The created vertex</returns>
        private RequestInsertVertex CreateRequest(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var result = new RequestInsertVertex(_TypeName);

            foreach (var aAttributeDefinition in _AttributeAssignList)
            {
                if (aAttributeDefinition is AttributeAssignOrUpdateValue)
                {
                    var value = aAttributeDefinition as AttributeAssignOrUpdateValue;

                    value.AttributeIDChain.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, true);
                }
            }

            return result;
        }

        #endregion

    }
}
