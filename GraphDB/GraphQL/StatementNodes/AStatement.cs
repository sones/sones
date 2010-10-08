/* <id name=”GraphLib – abstract statement class” />
 * <copyright file=”AStatement.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The abstract class for all statements in GraphDB.</summary>
 */

#region usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes
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

        protected Exceptional ParsingResult { get; private set; }

        #endregion

        #region abstract Methods

        #region abstract GetContent Method

        public abstract void GetContent(CompilerContext context, ParseTreeNode parseNode);

        #endregion

        #region abstract Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public abstract QueryResult Execute(IGraphDBSession graphDBSession);

        #endregion

        #endregion

        #region Ctor

        public AStatement()
        {
            ParsingResult = new Exceptional();
        } 

        #endregion
    
    }

}
