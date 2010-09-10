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

/* <id name="PandoraDB – PANDORADB CLI command" />
 * <copyright file="DBCLI_PANDORADB.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Starts and quits a given PandoraDB.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;

using sones.Lib.DataStructures;
using sones.GraphDB;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Starts and quits a given PandoraDB
    /// </summary>

    public class DBCLI_PANDORADB : AllBasicDBCLICommands
    {

        #region Constructor

        public DBCLI_PANDORADB()
        {

            // Command name and description
            InitCommand("PANDORADB",
                        "Starts and quits a given PandoraDB",
                        "Starts and quits a given PandoraDB");

            NonTerminal PANDORADB_Action = new NonTerminal("PANDORADB_Action");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + PANDORADB_Action);

            PANDORADB_Action.Rule = StartSymbol + stringLiteralPVFS
                                    | StopSymbol;
            //PANDORADB_Action.PandoraOptions.Add(PandoraOption.IsOption);
            
            _CommandNonTerminals.Add(PANDORADB_Action);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFS2Session, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IPandoraDBSession = myIPandoraDBSession as IGraphDBSession;

            if (_IGraphFS2Session == null || _IPandoraDBSession == null)
            {
                WriteLine("No OM database instance started...");
                return;
            }

            if (myOptions.ElementAt(1).Value[0].Option.Equals(StartSymbol.DisplayName))
            {
                //we have a start
                if (_IGraphFS2Session != null)
                {
                    if (_IPandoraDBSession == null)
                    {
                        var internalPandoraDB = new GraphDB2(new UUID(), new ObjectLocation(myOptions.ElementAt(2).Value[0].Option), _IGraphFS2Session, true);
                        var DB = new GraphDBSession(internalPandoraDB, _IGraphFS2Session.SessionToken.SessionInfo.Username);
                    }
                    else
                    {
                        Console.WriteLine("PandoraDB instance could not be started, because you already started a PandoraDB.");
                    }
                }
                else
                {
                    Console.WriteLine("PandoraDB instance could not be started, because you do not have a PandoraVFS mounted.");
                }
            }
            else
            {

                if (_IPandoraDBSession != null)
                {
                    _IPandoraDBSession.Shutdown();
                }
                else
                {
                    Console.WriteLine("No PandoraDB instance started...");
                }
            }
        }

        #endregion

    }

}
