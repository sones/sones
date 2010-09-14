/* GraphFS CLI - PANIC
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
using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDS.Connectors.CLI
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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            return Exceptional.OK;

        }

        #endregion

    }

}
