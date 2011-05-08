using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;
using System.Collections.Generic;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class DropTypeNode : AStatement, IAstNodeInit
    {
        #region Data

        //the name of the type that should be dropped
        String _TypeName = "";

        String _query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Name

            _TypeName = parseNode.ChildNodes[3].Token.ValueString;

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DropType"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _query = myQuery;

            return myGraphDB.DropType(mySecurityToken, myTransactionToken, new RequestDropVertexType(_TypeName), GenerateOutput);
        }

        #endregion

        #region helper

        private QueryResult GenerateOutput(IRequestStatistics myStats, Dictionary<Int64, String> myDeletedTypeIDs)
        {
            var temp = new Dictionary<String, object>();
            foreach(var item in myDeletedTypeIDs)
            {
                temp.Add("RemovedTypeID", item.Key);
                temp.Add("RemovedTypeName", item.Value);
            }
            return new QueryResult(_query, 
                                    "GQL", 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful, 
                                    new List<IVertexView> 
                                    { 
                                        new VertexView( temp, null )
                                    });
        }

        #endregion

    }
}
