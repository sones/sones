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


/* <id name="sones GraphDB – INSERT CLI command" />
 * <copyright file="DBCLI_INSERT.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Inserts new Objects into the PandoraDB</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using System.Text.RegularExpressions;
using sones.GraphFS.Session;
using sones.GraphDB;
using sones.GraphDB.QueryLanguage.Result;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Inserts new Objects into the PandoraDB
    /// </summary>

    public class DBCLI_INSERT : AllBasicDBCLICommands
    {

        #region Constructor

        public DBCLI_INSERT()
        {

            // Command name and description
            InitCommand("INSERT",
                        "Inserts new Objects into an instance of the PandoraDB",
                        "Inserts new Objects into an instance of the PandoraDB");

            #region BNF rule

            var INTO            = Symbol("INTO");
            var VALUES          = Symbol("VALUES");
            var AttrAssignList  = new NonTerminal("AttrAssignList");
            var AttrAssign      = new NonTerminal("AttrAssign");
            var StringOrNumber  = new NonTerminal("StringOrNumber");
            var comma           = Symbol(",");
            var gleich          = Symbol("=");
            var LISTOF          = Symbol("LISTOF");
            var SETOF           = Symbol("SETOF");
            var SETREF          = Symbol("SETREF");

            
            CreateBNFRule(CLICommandSymbolTerminal + INTO + stringLiteral + VALUES + "(" + AttrAssignList + ")");

            AttrAssignList.Rule = AttrAssign + comma + AttrAssignList
                                   | AttrAssign;

            AttrAssignList.PandoraOptions.Add(PandoraOption.IsStructuralObject);

            StringOrNumber.Rule = stringLiteral | numberLiteral;

            AttrAssign.Rule = stringLiteral + gleich + StringOrNumber
                                | stringLiteral + gleich + LISTOF + stringLiteral
                                | stringLiteral + gleich + SETOF + stringLiteral
                                | stringLiteral + gleich + SETREF + stringLiteral;

            AttrAssignList.PandoraOptions.Add(PandoraOption.IsOption);

            #endregion

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
                WriteLine("No database instance started...");
                return;
            }

            QueryResult _QueryResult;

            myInputString = Regex.Replace(myInputString, "[INSERT|insert]+ [INTO|into]+ (\')(.*?)(\') [VALUES|values]+", "INSERT INTO $2 VALUES");
            myInputString = Regex.Replace(myInputString, "([\\(|,]+)(\\s*)(\')(.*?)(\')(\\s*)=", "$1 $4 =");

            _QueryResult = QueryDB(myInputString, _IPandoraDBSession);


        }

        #endregion

    }

}
