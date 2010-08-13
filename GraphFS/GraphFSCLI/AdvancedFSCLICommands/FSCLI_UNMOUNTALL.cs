///* GraphFS CLI - UNMOUNTALL
// * (c) Henning Rauch, 2009
// * 
// * Unmounts all FS
// * 
// * Lead programmer:
// *      Henning Rauch
// *      Achim Friedland
// * 
// */

//#region Usings

//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using System.IO;

//using sones.Lib.Frameworks.CLIrony.Compiler;
//using sones.Graph.Storage.GraphFS;
//using sones.Graph.Storage.GraphFS.Objects;
//using sones.Lib;
//using sones.Lib.CLI
//using sones.Graph.Storage;
//using sones.Graph.Storage.GraphFS.Session;

//#endregion

//namespace sones.Graph.Applications.GraphFSCLI
//{

//    /// <summary>
//    /// Unmounts all FS
//    /// </summary>

//    public class FSCLI_UNMOUNTALL : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "UNMOUNTALL"; } }
//        public override String Command      { get { return "UNMOUNTALL"; } }
//        public override String ShortInfo    { get { return "Unmounts all GraphFS"; } }
//        public override String Information { get { return "Unmounts all GraphFS"; } }

//        public override CLICommandCategory aCategory
//        {
//            get { return CLICommandCategory.AdvancedFSCLICommand; }
//        }

//        #endregion

//        #region Command Grammar

//        public override Grammar CommandGrammar
//        {

//            get
//            {

//                _CommandGrammar.Terminals.AddRange(_CommandTerminals);


//                foreach (NonTerminal aNonTerminal in _CommandNonTerminals)
//                {
//                    if (!aNonTerminal.GraphOptions.Contains(GraphOption.IsOption))
//                    {
//                        aNonTerminal.GraphOptions.Add(GraphOption.IsStructuralObject);
//                    }
//                }

//                _CommandGrammar.NonTerminals.AddRange(_CommandNonTerminals);

//                foreach (SymbolTerminal _aCommandSymbolTerminal in _CommandSymbolTerminal)
//                {
//                    _CommandGrammar.Terminals.Add(_aCommandSymbolTerminal);
//                }

//                _CommandGrammar.Root = UNMOUNTALL;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_UNMOUNTALL()
//        {

//            #region BNF rules

//            UNMOUNTALL.Rule = UNMOUNTALL_CommandString;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(UNMOUNTALL);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(UNMOUNTALL_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal UNMOUNTALL_CommandString = Symbol("UNMOUNTALL");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal UNMOUNTALL = new NonTerminal("UNMOUNTALL");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {

//            IGraphFSSession MyPFS = myPFSObject as IGraphFSSession;

//            if (MyPFS != null)
//            {
//                MyPFS.UnmountAllFileSystems();
//                myCurrentPath = "[NA]";
//                myPFSObject = null;
//            }
//            else
//            {
//                WriteLine("nothing mounted...");
//            }

            
//        }

//        #endregion

//    }

//}
