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


/* PandoraFS CLI - PANIC
 * (c) Henning Rauch, 2009
 * 
 * The "PANIC"-command of the grammar-based Command Line
 * Interface.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Frameworks.CLIrony.Compiler;

#endregion

namespace sones.Lib.CLI
{

    /// <summary>
    /// Generates a lot of Panic!
    /// </summary>

    public class CLI_PANIC : AAllInBuildCLICommands
    {


        
        #region Constructor

        public CLI_PANIC()
        {

            // Command name and description
            InitCommand("PANIC",
                        "Generates a lot of Panic!",
                        "Generates a lot of Panic!");


            #region Symbol declaration

            SymbolTerminal PANIC_Option_panic_Symbol            = Symbol("--panic");
            SymbolTerminal PANIC_Option_more_panic_Symbol       = Symbol("--morePanic");
            SymbolTerminal PANIC_Option_infinite_panic_Symbol   = Symbol("--releaseTomorrow");
            SymbolTerminal PANIC_Option_longTest_Symbol         = Symbol("--long");

            #endregion

            #region Non-terminal declaration

            NonTerminal PANIC_Option_List           = new NonTerminal("PANIC_Option_List");
            NonTerminal PANIC_Option                = new NonTerminal("PANIC_Option");
            NonTerminal PANIC_Option_more_panic     = new NonTerminal("PANIC_Option_more_panic");
            NonTerminal PANIC_Option_infinite_panic = new NonTerminal("PANIC_Option_infinite_panic");
            NonTerminal PANIC_Option_longTest       = new NonTerminal("PANIC_Option_longTest");

            #endregion

            #region BNF rule

            PANIC_Option_List.Rule = PANIC_Option + PANIC_Option_List
                                    | PANIC_Option;

            PANIC_Option.Rule = PANIC_Option_panic_Symbol
                                    | PANIC_Option_more_panic
                                    | PANIC_Option_infinite_panic
                                    | PANIC_Option_longTest;

            PANIC_Option_more_panic.Rule = PANIC_Option_more_panic_Symbol;

            PANIC_Option_infinite_panic.Rule = PANIC_Option_infinite_panic_Symbol;

            PANIC_Option_longTest.Rule = PANIC_Option_longTest_Symbol;

            CreateBNFRule(CLICommandSymbolTerminal + PANIC_Option_List);

            #endregion

            #region Non-terminal integration

            _CommandNonTerminals.Add(PANIC_Option_List);
            _CommandNonTerminals.Add(PANIC_Option);
            _CommandNonTerminals.Add(PANIC_Option_more_panic);
            _CommandNonTerminals.Add(PANIC_Option_infinite_panic);
            _CommandNonTerminals.Add(PANIC_Option_longTest);

            #endregion

            #region Symbol integration

            _CommandSymbolTerminal.Add(PANIC_Option_panic_Symbol);
            _CommandSymbolTerminal.Add(PANIC_Option_more_panic_Symbol);
            _CommandSymbolTerminal.Add(PANIC_Option_infinite_panic_Symbol);
            _CommandSymbolTerminal.Add(PANIC_Option_longTest_Symbol);

            #endregion


        }

        #endregion


        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {
            _CancelCommand = false;
        }

        #endregion

    }

}
