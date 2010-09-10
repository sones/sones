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

/*
 * DBCLI_EXECDBSCRIPT
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphDB.GraphQL;
using sones.GraphDB.Structures;

using sones.GraphFS.Session;
using sones.GraphDS.Connectors.CLI;
using sones.GraphDBInterface.Result;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Executes a GraphDB script
    /// </summary>

    public class DBCLI_EXECDBSCRIPT : AllBasicDBCLICommands
    {
        
        #region Constructor

        public DBCLI_EXECDBSCRIPT()
        {

            // Command name and description
            InitCommand("EXECDBSCRIPT",
                        "Executes a GraphDB script",
                        "Executes a GraphDB script");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralExternalEntry);

            this.OnCancelRequested += new EventHandler(DBCLI_EXECDBSCRIPT_OnCancelRequested);

        }

        void DBCLI_EXECDBSCRIPT_OnCancelRequested(object sender, EventArgs e)
        {
            _CancelCommand = true;
        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            var gqlQuery = new GraphQLQuery(myAGraphDSSharp.IGraphDBSession.DBPluginManager);

            Boolean isSuccessful = true;
            int numberOfStatement = 1;
            Stopwatch sw = new Stopwatch();
            String ScriptFile = myOptions.ElementAt(1).Value[0].Option;

            //((FSSessionInfo)_IGraphFS2Session.SessionToken.SessionInfo).FSSettings.AutoCommitAfterNumberOfPendingTransactions = 1000;

            String line = "";
            try
            {
                StreamReader reader = new StreamReader(ScriptFile);

                //_IGraphFS2Session.BeginTransaction();

                sw.Start();
                while ((line = reader.ReadLine()) != null)
                {
                    if (!ExecuteAQuery(line.Trim(), myAGraphDSSharp.IGraphDBSession, gqlQuery))
                    {
                        WriteLine("Error while executing query: \"" + line + "\"");

                        isSuccessful= false;
                        break;
                    }

                    if (line.Length != 0)
                    {
                        numberOfStatement++;
                    }

                    if (_CancelCommand)
                    {
                        Console.WriteLine("... parsing stopped.");
                        break;
                    }

                }

                sw.Stop();
                reader.Close();


            }
            catch
            {
                // Fehler beim Öffnen der Datei
            }

            if (isSuccessful)
            {
                WriteLine("\nSuccessfully executed " + numberOfStatement + " statements in " + sw.Elapsed.Seconds + " seconds.");
                //_IGraphFS2Session.CommitTransaction();
            }
            else
            {
                //_IGraphFS2Session.RollbackTransaction();
            }

            return Exceptional.OK;

        }

        private bool ExecuteAQuery(string query, IGraphDBSession dbSession, GraphQLQuery myGQLQuery)
        {
            if (query.Length != 0)
            {
                //execute query
                QueryResult myQueryResult = myGQLQuery.Query(query, dbSession);

                WriteLine(query);

                HandleQueryResult(myQueryResult, false);

                if (myQueryResult.ResultType != ResultType.Successful)
                {
                    WriteLine(myQueryResult.GetErrorsAsString());
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        #endregion

    }

}
