using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request.RebuildIndices;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class RebuildIndicesNode : AStatement, IAstNodeInit
    {
        #region Data

        private HashSet<String> _Types;
        private String Query;
        private IEnumerable<IIndexDefinition> IndexDefinitions;

        #endregion

        #region constructors

        public RebuildIndicesNode()
        { }

        #endregion
        
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _Types = new HashSet<string>();

            if (parseNode.ChildNodes[2].ChildNodes != null && parseNode.ChildNodes[2].ChildNodes.Count != 0)
            {
                parseNode.ChildNodes[2].ChildNodes[0].ChildNodes.ForEach(item => _Types.Add(((ATypeNode)item.AstNode).ReferenceAndType.TypeName));
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "REBUILD INDICES"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            QueryResult qresult = null;
            Query = myQuery;

            try
            {
                var request = new RequestRebuildIndices(_Types);

                qresult = myGraphDB.RebuildIndices<QueryResult>(mySecurityToken, myTransactionToken, request, GenerateOutput);
            }
            catch (ASonesException e)
            {
                qresult.Error = e;
            }

            return qresult;
        }

        #endregion

        private QueryResult GenerateOutput(IRequestStatistics myStats, IEnumerable<IIndexDefinition> myIndexDefs)
        {
            return new QueryResult(Query,
                                    "sones.gql",
                                    Convert.ToUInt64(myStats.ExecutionTime),
                                    ResultType.Successful,
                                    new List<IVertexView> { new VertexView(new Dictionary<String, object> { {"CreatedIndex", IndexDefinitions} } , 
                                                                            new Dictionary<String, IEdgeView>()) });
        }
    }
}
