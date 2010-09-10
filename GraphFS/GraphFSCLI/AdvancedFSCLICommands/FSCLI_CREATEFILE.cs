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

///* PandoraFS CLI - CREATEFILE
// * (c) Henning Rauch, 2009
// * 
// * Imports a file into a pandora file system
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
//using sones.Pandora.Storage.PandoraFS;
//using sones.Pandora.Storage.PandoraFS.Objects;
//using sones.Lib.CLI
//using sones.Pandora.Storage;
//using sones.Pandora.Storage.PandoraFS.Session;
//using sones.Pandora.Storage.PandoraFS.Datastructures;

//#endregion

//namespace sones.Pandora.Applications.PandoraFSCLI
//{

//    /// <summary>
//    /// Imports a file into a pandora file system
//    /// </summary>

//    public class FSCLI_CREATEFILE : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "CREATEFILE"; } }
//        public override String Command { get { return "CREATEFILE"; } }
//        public override String ShortInfo { get { return "Creates a file from the host system in the PandoraFS."; } }
//        public override String Information { get { return "Creates a file from the host system in the PandoraFS."; } }

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
//                    if (!aNonTerminal.PandoraOptions.Contains(PandoraOption.IsOption))
//                    {
//                        aNonTerminal.PandoraOptions.Add(PandoraOption.IsStructuralObject);
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
//            String FileWithinPandoraFS = myOptions.ElementAt(2).Value[0].Option;

//            //TODO: Comparism of FS and File size
//            FileStream ToBeTransformedFile = new FileStream(FileOnHostSystem, FileMode.Open);
//            byte[] ToBeTransformedBytes = new byte[ToBeTransformedFile.Length];

//            ToBeTransformedFile.Read(ToBeTransformedBytes, 0, ToBeTransformedBytes.Length);
//            ToBeTransformedFile.Close();

//            int FileLength = FileWithinPandoraFS.Length;

//            APandoraObject _FileObject = new FileObject(FileWithinPandoraFS.Remove(FileLength - 1), ToBeTransformedBytes, true);
//            ((IGraphFSSession)myPFSObject).StoreAPandoraObject(ref _FileObject, true);

//        }

//        #endregion

//    }

//}
