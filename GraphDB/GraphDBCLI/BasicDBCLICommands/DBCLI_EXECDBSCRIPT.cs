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


/* <id name="sones GraphDB – CLI" />
 * <copyright file="DBCLI_EXECDBSCRIPT.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Björn Elmar Macek</developer>
 * <summary>Executes a PandoraDB script</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.ErrorHandling;
using System.IO;
using System.Diagnostics;

using sones.GraphDB;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Executes a PandoraDB script
    /// </summary>

    public class DBCLI_EXECDBSCRIPT : AllBasicDBCLICommands
    {
        
        #region Constructor

        public DBCLI_EXECDBSCRIPT()
        {

            // Command name and description
            InitCommand("EXECDBSCRIPT",
                        "Executes a PandoraDB script",
                        "Executes a PandoraDB script");

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

        public override void Execute(ref object myIGraphFS2Session, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {
            
            var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IPandoraDBSession = myIPandoraDBSession as IGraphDBSession;

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
                    if (!ExecuteAQuery(line.Trim(), _IPandoraDBSession))
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
        }

        private bool ExecuteAQuery(string query, IGraphDBSession dbSession)
        {
            if (query.Length != 0)
            {
                //execute query
                QueryResult myQueryResult = dbSession.Query(query);

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
