using System;
using System.Collections.Generic;
using sones.GraphQL.StructureNodes;
using Irony.Parsing;
using sones.GraphQL.Structures.Enums;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphQL.Result;

namespace sones.GraphQL.StatementNodes
{

    /// <summary>
    /// The abstract class for all statements in GraphDB.
    /// </summary>
    public abstract class AStatement : AStructureNode
    {

        #region Data


        //NLOG: temporarily commented
        //protected static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region General Command Infos

        public abstract String StatementName { get; }
        public abstract TypesOfStatements TypeOfStatement { get; }

        #endregion

        #region Protected properties

        //protected Exceptional ParsingResult { get; private set; }

        #endregion

        #region abstract Methods

        #region abstract GetContent Method

        public abstract void GetContent(ParsingContext context, ParseTreeNode parseNode);

        #endregion

        #region abstract Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public abstract QueryResult Execute(SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        #endregion

        #endregion
                

    }

}
