using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using System.Diagnostics;
using sones.Library.ErrorHandling;
using System.Collections.Generic;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DescribeNode : AStatement, IAstNodeInit
    {
        #region Data

        private ADescribeDefinition _DescribeDefinition;

        #endregion

        #region Constructor

        public DescribeNode()
        {
        }

        #endregion        

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DescribeDefinition = ((ADescrNode)parseNode.ChildNodes[1].AstNode).DescribeDefinition;
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Describe"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            QueryResult qresult = null;
            ASonesException error = null;

            try
            {
                qresult = _DescribeDefinition.GetResult(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
            }
            catch (ASonesException e)
            {
                error = e;
            }

            sw.Stop();

            return new QueryResult(myQuery, "sones.gql", (ulong)sw.ElapsedMilliseconds, qresult != null ? qresult.TypeOfResult : ResultType.Failed, qresult != null ? qresult.Vertices : new IVertexView[0], error);
        }

        #endregion
    }
}
