///* GraphFS CLI - CREATEFILE
// * (c) Henning Rauch, 2009
// * 
// * Imports a file into a Graph file system
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
//using sones.Lib.CLI
//using sones.Graph.Storage;
//using sones.Graph.Storage.GraphFS.Session;
//using sones.Graph.Storage.GraphFS.Datastructures;

//#endregion

//namespace sones.Graph.Applications.GraphFSCLI
//{

//    /// <summary>
//    /// Imports a file into a Graph file system
//    /// </summary>

//    public class FSCLI_CREATEFILE : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "CREATEFILE"; } }
//        public override String Command { get { return "CREATEFILE"; } }
//        public override String ShortInfo { get { return "Creates a file from the host system in the GraphFS."; } }
//        public override String Information { get { return "Creates a file from the host system in the GraphFS."; } }

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

//                _CommandGrammar.Root = CREATEFILE;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_CREATEFILE()
//        {

//            #region BNF rules

//            CREATEFILE.Rule = CREATEFILE_CommandString + stringLiteralExternalEntry + stringLiteralPVFS;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(CREATEFILE);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(CREATEFILE_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal CREATEFILE_CommandString = Symbol("CREATEFILE");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal CREATEFILE = new NonTerminal("CREATEFILE");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {

//            String FileOnHostSystem    = myOptions.ElementAt(1).Value[0].Option;
//            String FileWithinGraphFS = myOptions.ElementAt(2).Value[0].Option;

//            //TODO: Comparism of FS and File size
//            FileStream ToBeTransformedFile = new FileStream(FileOnHostSystem, FileMode.Open);
//            byte[] ToBeTransformedBytes = new byte[ToBeTransformedFile.Length];

//            ToBeTransformedFile.Read(ToBeTransformedBytes, 0, ToBeTransformedBytes.Length);
//            ToBeTransformedFile.Close();

//            int FileLength = FileWithinGraphFS.Length;

//            AGraphObject _FileObject = new FileObject(FileWithinGraphFS.Remove(FileLength - 1), ToBeTransformedBytes, true);
//            ((IGraphFSSession)myPFSObject).StoreAGraphObject(ref _FileObject, true);

//        }

//        #endregion

//    }

//}
