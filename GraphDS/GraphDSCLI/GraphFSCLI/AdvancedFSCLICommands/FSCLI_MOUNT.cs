///* GraphFS CLI - MOUNT
// * (c) Henning Rauch, 2009
// * 
// * Mounts a new file system
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
//    /// Mounts a file system
//    /// </summary>

//    public class FSCLI_MOUNT : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "MOUNT"; } }
//        public override String Command      { get { return "MOUNT"; } }
//        public override String ShortInfo    { get { return "Mounts a given GraphFS into a directory."; } }
//        public override String Information  { get { return "Mounts a given GraphFS into a directory."; } }

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

//                _CommandGrammar.Root = MOUNT;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_MOUNT()
//        {

//            #region BNF rules

//            MOUNT.Rule = MOUNT_CommandString + Protocol + stringLiteralExternalEntry + stringLiteralPVFS;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(MOUNT);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(MOUNT_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal MOUNT_CommandString = Symbol("MOUNT");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal MOUNT = new NonTerminal("MOUNT");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {
//            String Filename;
//            String Mountpoint;

//            Filename = myOptions.ElementAt(1).Value[0].Option + myOptions.ElementAt(2).Value[0].Option;
//            Mountpoint = myOptions.ElementAt(3).Value[0].Option;

//            if (File.Exists(Filename.Substring(7)))
//            {
//                if (myPFSObject == null)
//                {
//                    myPFSObject = new GraphFS();
//                }
//                ((IGraphFSSession)myPFSObject).MountFileSystem(Filename, Mountpoint);

//                if (((IGraphFSSession)myPFSObject).isIDirectoryObject(Mountpoint))
//                {
//                    if (Mountpoint.Equals(FSPathConstants.PathDelimiter))
//                    {
//                        myCurrentPath = FSPathConstants.PathDelimiter;
//                    }
//                }
//                else
//                {
//                    if (Mountpoint.Equals(FSPathConstants.PathDelimiter))
//                    {
//                        myPFSObject = null;
//                    }
//                }
//            }
//            else
//            {
//                WriteLine("File does not exists. Aborting.");
//            }

//        }

//        #endregion

//    }

//}
