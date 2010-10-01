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
 * DBCLI_CREATE
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

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
                        "Inserts new Objects into an instance of the GraphDB or creates an index on attributes of a type",
                        "Inserts new Objects into an instance of the GraphDB or creates an index on attributes of a type");

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

            EXTENDS_Action.Rule = EXTENDSSymbol + GraphTypeNT + ATTRIBUTES;

            ATTRIBUTES.Rule = ATTRIBUTES_Symbol + BracketRoundOpenSymbol + ATTRIBUTE_list;

            ATTRIBUTE_list.Rule = GraphTypeNT + stringLiteral + CommaSymbol + ATTRIBUTE_list
                                | GraphTypeNT + stringLiteral + BracketRoundCloseSymbol;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            var _QueryString = myInputString.Replace("'", "");

            HandleQueryResult(QueryDB(myAGraphDSSharp, _QueryString), true);

            return Exceptional.OK;

        }

        #endregion

    }

}
