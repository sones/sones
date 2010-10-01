/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – CommitRollbackTransactionNode" />
 * <copyright file="CommitRollbackTransactionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;
using sones.GraphDB.Transactions;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Transaction
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
        /// <param name="myGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myGraphDBSession)
        {

            DBTransaction _GraphDBTransaction;

            if (CommandType == Transaction.CommandType.Commit)
            {
                _GraphDBTransaction = myGraphDBSession.CommitTransaction();
            }

            else
            {
                _GraphDBTransaction = myGraphDBSession.RollbackTransaction();
            }

            if (_GraphDBTransaction.Success())
            {

                var _ReturnValues = new Dictionary<String, Object>();
                _ReturnValues.Add("UUID",           _GraphDBTransaction.UUID);
                _ReturnValues.Add("Created",        _GraphDBTransaction.Created);
                _ReturnValues.Add("Finished",       _GraphDBTransaction.Finished);
                _ReturnValues.Add("Distributed",    _GraphDBTransaction.Distributed);
                _ReturnValues.Add("IsolationLevel", _GraphDBTransaction.IsolationLevel);
                _ReturnValues.Add("LongRunning",    _GraphDBTransaction.LongRunning);
                _ReturnValues.Add("Name",           _GraphDBTransaction.Name);
                _ReturnValues.Add("State",          _GraphDBTransaction.State);

                return new QueryResult(new Vertex(_ReturnValues)).PushIExceptional(ParsingResult);

            }

            return new QueryResult(_GraphDBTransaction).PushIExceptional(ParsingResult);

        }

        #endregion

    }

}
