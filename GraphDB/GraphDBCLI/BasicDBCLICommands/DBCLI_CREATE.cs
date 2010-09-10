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

/* <id name="PandoraDB – CREATE CLI command" />
 * <copyright file="DBCLI_CREATETYPE.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Creates new Types</summary>
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
using sones.GraphDB.QueryLanguage.Result;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Creates new Types
    /// </summary>

    public class DBCLI_CREATE : AllBasicDBCLICommands
    {

        #region Constructor

        public DBCLI_CREATE()
        {

            // Command name and description
            InitCommand("CREATE",
                        "Inserts new Objects into an instance of the PandoraDB or creates an index on attributes of a type",
                        "Inserts new Objects into an instance of the PandoraDB or creates an index on attributes of a type");

            #region BNF rule

            #region Symbol declaration

            SymbolTerminal CREATETYPE_CommandString = Symbol("CREATE");
            SymbolTerminal ATTRIBUTES_Symbol = Symbol("ATTRIBUTES");
            SymbolTerminal EXTENDSSymbol = Symbol("EXTENDS");

            #endregion

            #region Non-terminal declaration

            NonTerminal CREATETYPE = new NonTerminal("CREATETYPE");
            NonTerminal CREATETYPE_Action = new NonTerminal("CREATETYPE_Action");
            NonTerminal EXTENDS_Action = new NonTerminal("EXTENDS_Action");
            NonTerminal ATTRIBUTES = new NonTerminal("ATTRIBUTES");
            NonTerminal ATTRIBUTE_list = new NonTerminal("ATTRIBUTE_list");
            NonTerminal Id = new NonTerminal("Id");

            #endregion


            CreateBNFRule(CLICommandSymbolTerminal + TYPESymbol + stringLiteral + CREATETYPE_Action);

            CREATETYPE_Action.Rule = EXTENDS_Action
                                        | ATTRIBUTES;

            EXTENDS_Action.Rule = EXTENDSSymbol + PandoraTypeNT + ATTRIBUTES;

            ATTRIBUTES.Rule = ATTRIBUTES_Symbol + BracketRoundOpenSymbol + ATTRIBUTE_list;

            ATTRIBUTE_list.Rule = PandoraTypeNT + stringLiteral + CommaSymbol + ATTRIBUTE_list
                                | PandoraTypeNT + stringLiteral + BracketRoundCloseSymbol;

            #region Non-terminal integration

            _CommandNonTerminals.Add(CREATETYPE);
            _CommandNonTerminals.Add(CREATETYPE_Action);
            _CommandNonTerminals.Add(EXTENDS_Action);
            _CommandNonTerminals.Add(ATTRIBUTES);
            _CommandNonTerminals.Add(ATTRIBUTE_list);
            _CommandNonTerminals.Add(Id);

            #endregion

            #region Symbol integration

            _CommandSymbolTerminal.Add(CREATETYPE_CommandString);
            _CommandSymbolTerminal.Add(ATTRIBUTES_Symbol);
            _CommandSymbolTerminal.Add(EXTENDSSymbol);

            #endregion

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

            if (_IPandoraDBSession != null)
            {
                String QueryInputString = myInputString.Replace("'", "");

                HandleQueryResult(_IPandoraDBSession.Query(QueryInputString), true);
            }
            else
            {
                Console.WriteLine("No PandoraDB instance started...");
            }

        }

        #endregion

    }

}
