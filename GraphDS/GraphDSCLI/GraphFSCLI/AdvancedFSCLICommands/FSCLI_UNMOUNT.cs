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

///* GraphFS CLI - UNMOUNT
// * (c) Henning Rauch, 2009
// * 
// * Unmounts a file system
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
//    /// Unmounts a file system
//    /// </summary>

//    public class FSCLI_UNMOUNT : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "UNMOUNT"; } }
//        public override String Command      { get { return "UNMOUNT"; } }
//        public override String ShortInfo    { get { return "Unmounts a given mountpoint of a GraphFS"; } }
//        public override String Information { get { return "Unmounts a given mountpoint of a GraphFS"; } }

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

//                _CommandGrammar.Root = UNMOUNT;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_UNMOUNT()
//        {

//            #region BNF rules

//            UNMOUNT.Rule = UNMOUNT_CommandString + stringLiteralPVFS;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(UNMOUNT);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(UNMOUNT_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal UNMOUNT_CommandString = Symbol("UNMOUNT");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal UNMOUNT = new NonTerminal("UNMOUNT");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {
//            String Mountpoint;

//            Mountpoint = myOptions.ElementAt(1).Value[0].Option;
//            ((IGraphFSSession)myPFSObject).UnmountFileSystem(Mountpoint);

//            if (Mountpoint.Equals(FSPathConstants.PathDelimiter))
//            {
//                myCurrentPath = "[NA]";
//            }
//        }

//        #endregion

//    }

//}
