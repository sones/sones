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

///* PandoraFS CLI - FILEEXISTS
// * (c) Henning Rauch, 2009
// * 
// * Checks if a file object exists
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
//using sones.Lib;
//using sones.Lib.CLI
//using sones.Pandora.Storage;
//using sones.Pandora.Storage.PandoraFS.Session;

//#endregion

//namespace sones.Pandora.Applications.PandoraFSCLI
//{

//    /// <summary>
//    /// Checks if a file object exists
//    /// </summary>

//    public class FSCLI_FILEEXISTS : AScriptingFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "FILEEXISTS"; } }
//        public override String Command      { get { return "FILEEXISTS"; } }
//        public override String ShortInfo    { get { return "Checks if a certain file exists."; } }
//        public override String Information  { get { return "Checks if a certain file exists."; } }

//        public override CLICommandCategory aCategory
//        {
//            get { return CLICommandCategory.ScriptingFSCLICommand; }
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

//                _CommandGrammar.Root = FILEEXISTS;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_FILEEXISTS()
//        {

//            #region BNF rules

//            FILEEXISTS.Rule = FILEEXISTS_CommandString + stringLiteralPVFS;
            
//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(FILEEXISTS);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(FILEEXISTS_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal FILEEXISTS_CommandString = Symbol("FILEEXISTS");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal FILEEXISTS = new NonTerminal("FILEEXISTS");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {
//            WriteLine(((IGraphFSSession)myPFSObject).ObjectExists(GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option)));
//        }

//        #endregion

//    }

//}
