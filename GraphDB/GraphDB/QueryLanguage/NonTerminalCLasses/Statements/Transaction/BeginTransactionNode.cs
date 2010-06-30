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


/* <id name="sones GraphDB – BeginTransactionNode" />
 * <copyright file="BeginTransactionNode.cs"
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
using sones.GraphDB.QueryLanguage.Enums;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Session;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphFS.Transactions;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Errors.Transactions;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Transaction
{
    public class BeginTransactionNode : AStatement
    {
        #region data

        private Boolean         _IsDistributed;
        private Boolean         _IsLongRunning;
        private IsolationLevel  _Isolation;
        private String          _Name;
        private DateTime?       _TimeStamp = null;

        #endregion

        #region AStatement

        #region properties

        public override string StatementName
        {
            get { return "BeginTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        #endregion

        #region constructors

        public BeginTransactionNode()
        { }

        #endregion

        #region private helper methods

        /// <summary>
        /// get transaction options like distributed or long-running
        /// </summary>
        /// <param name="parseNode">the node that contain the values</param>
        private void GetTransactOptions(ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[1] != null)
            {
                if (parseNode.ChildNodes[1].HasChildNodes())
                {
                    if (parseNode.ChildNodes[1].ChildNodes[0] != null)
                    {
                        switch (parseNode.ChildNodes[1].ChildNodes[0].Token.Text.ToUpper())
                        {
                            case DBConstants.TRANSACTION_DISTRIBUTED:
                                _IsDistributed = true;
                                break;

                            case DBConstants.TRANSACTION_LONGRUNNING:
                                _IsLongRunning = true;
                                break;
                        }
                    }

                    if (parseNode.ChildNodes[1].ChildNodes.Count > 1)
                    {
                        if (parseNode.ChildNodes[1].ChildNodes[1].Token.Text.ToUpper() == DBConstants.TRANSACTION_LONGRUNNING)
                            _IsLongRunning = true;
                    }
                }
            }
        }
                
        /// <summary>
        /// get the values for isolation, name or timestamp
        /// </summary>
        /// <param name="parseNode">the child node that contain the values</param>
        /// <param name="myCurrentChildNode">current child node</param>
        private void GetTransactAttributes(ParseTreeNode parseNode, Int32 myCurrentChildNode)
        {
            if (myCurrentChildNode < parseNode.ChildNodes.Count)
            {
                if (parseNode.ChildNodes[myCurrentChildNode].HasChildNodes())
                {
                    switch (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper())
                    {
                        case DBConstants.TRANSACTION_ISOLATION :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                if (!Enum.TryParse<IsolationLevel>(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true, out _Isolation))
                                {
                                    throw new GraphDBException(new Error_InvalidTransactionIsolationLevel(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString));
                                }
                                //_Isolation = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true);
                            }
                            break;
                        
                        case DBConstants.TRANSACTION_NAME :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                _Name = parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                            }
                            break;
                        
                        case DBConstants.TRANSACTION_TIMESTAMP :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                _TimeStamp = DateTime.ParseExact(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, "yyyyddMM.HHmmss.fffffff", null);
                            }
                            break;
                    }

                }

                GetTransactAttributes(parseNode, myCurrentChildNode + 1);
            }            
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            try
            {                
                if (parseNode.HasChildNodes())
                {
                    GetTransactOptions(parseNode);

                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes.Count > 3)
                    {
                        GetTransactAttributes(parseNode.ChildNodes[3], 0);
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
            var dbTransaction = graphDBSession.BeginTransaction(_IsDistributed, _IsLongRunning, _Isolation, _Name, _TimeStamp);

            if (dbTransaction.Success)
            {
                var readoutVals = new Dictionary<String, Object>();
                readoutVals.Add("UUID", dbTransaction.UUID.ToHexString());
                readoutVals.Add("Created", dbTransaction.Created);
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

        #region Accessor

        public Boolean IsDistributed
        { get { return _IsDistributed; } }

        public Boolean IsLongRunning
        { get { return _IsLongRunning; } }

        public IsolationLevel Isolation
        { get { return _Isolation; } }

        public String Name
        { get { return _Name; } }

        public DateTime? TimeStamp
        { get { return _TimeStamp; } }

        #endregion
    }
}
