/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="sones GraphDB – CommitRollbackTransactionNode" />
 * <copyright file="CommitRollbackTransactionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.Lib.Session;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Transactions;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Transaction
{
    public enum CommandType
    { 
        Commit,
        Rollback
    }
    
    public class CommitRollbackTransactionNode : AStatement
    {

        #region Properties

        public String Name { get; private set; }
        public Boolean ASync { get; private set; }
        public CommandType CommandType { get; private set; }

        #endregion

        #region Constructor

        public CommitRollbackTransactionNode()
        { }

        #endregion

        #region private helper methods
        
        /// <summary>
        /// get values for name and async
        /// </summary>
        /// <param name="myNode">the child node that contain the values</param>
        /// <param name="myCurrentChildNode">the current child node</param>
        private void GetAttributes(ParseTreeNode myNode, Int32 myCurrentChildNode)
        {
            if (myCurrentChildNode < myNode.ChildNodes.Count)
            {
                if (myNode.ChildNodes[myCurrentChildNode].HasChildNodes())
                {
                    if (myNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper() == DBConstants.TRANSACTION_NAME)
                        Name = myNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                }
                else
                {
                    if (myNode.ChildNodes[myCurrentChildNode].Token.Text.ToUpper() == DBConstants.TRANSACTION_COMROLLASYNC)
                        ASync = true;
                }
            
                GetAttributes(myNode, myCurrentChildNode + 1);    
            }        
        }

        #endregion


        #region AStatement

        public override string StatementName
        {
            get { return "CommitRollbackTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            try
            {
                if (parseNode.HasChildNodes())
                {
                    CommandType = (CommandType)Enum.Parse(typeof(CommandType), parseNode.ChildNodes[0].ChildNodes[0].Token.Text, true);
                    
                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes[2].HasChildNodes())
                    {
                        GetAttributes(parseNode.ChildNodes[2], 0);    
                    }
                }
            }
            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {
            var qr = new QueryResult();
            DBTransaction dbTransaction;
            if (CommandType == Transaction.CommandType.Commit)
            {
                dbTransaction = graphDBSession.CommitTransaction();

            }
            else
            {
                dbTransaction = graphDBSession.RollbackTransaction();
            }

            if (dbTransaction.Success)
            {
                var readoutVals = new Dictionary<String, Object>();
                readoutVals.Add("UUID", dbTransaction.UUID.ToHexString());
                readoutVals.Add("Created", dbTransaction.Created);
                readoutVals.Add("Finished", dbTransaction.Finished);
                readoutVals.Add("Distributed", dbTransaction.Distributed);
                readoutVals.Add("IsolationLevel", dbTransaction.IsolationLevel.ToString());
                readoutVals.Add("LongRunning", dbTransaction.LongRunning);
                readoutVals.Add("Name", dbTransaction.Name);
                readoutVals.Add("State", dbTransaction.State.ToString());

                var selResultSet = new SelectionResultSet(new DBObjectReadout(readoutVals));
                qr.AddResult(selResultSet);
            }
            else
            {
                qr.AddErrorsAndWarnings(dbTransaction);
            }

            return qr;
        }

        #endregion

    }
}
