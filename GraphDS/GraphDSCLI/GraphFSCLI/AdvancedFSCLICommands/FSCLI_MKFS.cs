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

///* GraphFS CLI - MKFS
// * (c) Henning Rauch, 2009
// * 
// * Creates a new file system
// * 
// * Lead programmer:
// *      Henning Rauch
// * 
// */

//#region Usings

//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;

//using sones.Lib.Frameworks.CLIrony.Compiler;
//using sones.Graph.Storage.GraphFS;
//using sones.Graph.Storage.GraphFS.Objects;
//using sones.Lib.CLI
//using sones.Graph.Storage;
//using sones.Graph.Storage.GraphFS.Session;

//#endregion

//namespace sones.Graph.Applications.GraphFSCLI
//{

//    /// <summary>
//    /// Creates a new file system
//    /// </summary>

//    public class FSCLI_MKFS : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "MKFS"; } }
//        public override String Command      { get { return "MAKEFILESYSTEM"; } }
//        public override String ShortInfo    { get { return "Creates a file system."; } }
//        public override String Information  { get { return "Creates a file system."; } }

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

//                _CommandGrammar.Root = MKFS;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_MKFS()
//        {

//            #region BNF rules

//            MKFS.Rule = MKFS_CommandString + Protocol + stringLiteralExternalEntry + numberLiteral + MultiplicatorSize + stringLiteral;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(MKFS);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(MKFS_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal MKFS_CommandString = Symbol("MKFS");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal MKFS = new NonTerminal("MKFS");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {
//            String Filename;
//            UInt64 FSSize;
//            String FSName;

//            IGraphFS        _IGraphFS         = new GraphFS();
//            IGraphFSSession _IGraphFSSession  = new GraphFSSession(_IGraphFS, "root");

//            FSName = myOptions.ElementAt(5).Value[0].Option;
//            Filename = myOptions.ElementAt(1).Value[0].Option + myOptions.ElementAt(2).Value[0].Option;
//            FSSize = GetBytesFromString(myOptions.ElementAt(3).Value[0].Option + myOptions.ElementAt(4).Value[0].Option);

//            _IGraphFSSession.MakeFileSystem(Filename, FSName, FSSize, false);
//            WriteLine("New file system created.");
//        }

//        #endregion

//        private ulong GetBytesFromString(string SizeInBytesAsString)
//        {
//            ulong FSSize = Convert.ToUInt64(SizeInBytesAsString.Remove(SizeInBytesAsString.Length - 1));

//            if (SizeInBytesAsString.ToUpper()[SizeInBytesAsString.Length - 1] == 'G')
//                FSSize = FSSize * 1024 * 1024 * 1024;

//            if (SizeInBytesAsString.ToUpper()[SizeInBytesAsString.Length - 1] == 'K')
//                FSSize = FSSize * 1024;

//            if (SizeInBytesAsString.ToUpper()[SizeInBytesAsString.Length - 1] == 'M')
//                FSSize = FSSize * 1024 * 1024;

//            return FSSize;
//        }

//    }

//}
