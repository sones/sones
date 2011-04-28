using System;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.StatementNodes
{
    /// <summary>
    /// The abstract class for all statements in GraphDB.
    /// </summary>
    public abstract class AStatement : AStructureNode
    {

        #region General Command Infos

        public abstract String StatementName { get; }
        public abstract TypesOfStatements TypeOfStatement { get; }

        #endregion

        #region abstract Methods

        #region abstract Execute

        public abstract QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        #endregion

        #endregion
    }
}
