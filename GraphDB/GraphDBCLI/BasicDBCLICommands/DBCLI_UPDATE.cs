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


/* <id name="sones GraphDB – UPDATE CLI command" />
 * <copyright file="DBCLI_UPDATE.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Updates myAttributes of certain objects.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.GraphFS.Session;
using sones.GraphDB;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Updates myAttributes of certain objects.
    /// </summary>

    public class DBCLI_UPDATE : AllBasicDBCLICommands
    {
        
        #region Constructor

        public DBCLI_UPDATE()
        {

            // Command name and description
            InitCommand("UPDATE",
                        "Updates myAttributes of certain objects",
                        "Updates myAttributes of certain objects");

            #region Symbol declaration

            SymbolTerminal ADDREFSymbol = Symbol("ADDREF");
            SymbolTerminal REMREFSymbol = Symbol("REMREF");

            #endregion

            #region Non-terminal declaration

            NonTerminal AttrUpdateList = new NonTerminal("AttrUpdateList");
            NonTerminal AttrUpdate = new NonTerminal("AttrUpdate");
            NonTerminal WhereOrValuesAction = new NonTerminal("WhereOrValuesAction");

            #endregion

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + TYPESymbol + AType + WhereOrValuesAction);

            WhereOrValuesAction.Rule = WhereClauseNT + ValuesSymbol + BracketRoundOpenSymbol + AttrUpdateList
                                        | ValuesSymbol + BracketRoundOpenSymbol + AttrUpdateList;

            AttrUpdateList.Rule = AttrUpdate + CommaSymbol + AttrUpdateList
                                        | AttrUpdate + BracketRoundCloseSymbol;

            AttrUpdate.Rule = AttrAssignNT
                                | IdNT + ADDREFSymbol + GraphTypeNT + UUIDSymbol + Eq_Equals + stringLiteral
                                | IdNT + REMREFSymbol + GraphTypeNT + UUIDSymbol + Eq_Equals + stringLiteral;


            #region Non-terminal integration

            _CommandNonTerminals.Add(AttrUpdateList);
            _CommandNonTerminals.Add(AttrUpdate);
            _CommandNonTerminals.Add(WhereOrValuesAction);

            #endregion


        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFS2Session, ref object myIGraphDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IGraphDBSession = myIGraphDBSession as IGraphDBSession;

            if (_IGraphFS2Session == null || _IGraphDBSession == null)
            {
                WriteLine("No OM database instance started...");
                return;
            }

            var QueryInputString = myInputString.Replace("'", "");

            HandleQueryResult(QueryDB(QueryInputString, _IGraphDBSession), true);
           
        }

        #endregion

    }

}
