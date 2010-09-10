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

///* PandoraFS CLI - MKDIR
// * (c) Henning Rauch, 2009
// * 
// * Creates a new directory
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

//using sones.Lib.Frameworks.CLIrony.Compiler;
//using sones.Pandora.Storage.PandoraFS;
//using sones.Pandora.Storage.PandoraFS.Objects;
//using sones.Notifications;
//using sones.Lib.CLI
//using sones.Pandora.Storage;
//using sones.Pandora.Storage.PandoraFS.Session;

//#endregion

//namespace sones.Pandora.Applications.PandoraFSCLI
//{

//    /// <summary>
//    /// Creates a new directory
//    /// </summary>

//    public class FSCLI_NOTIFICATION : AAdvancedFSCLICommands
//    {


//        #region Properties

//        #region General Command Infos

//        public override String ShortCommand { get { return "NOTIFICATION"; } }
//        public override String Command      { get { return "NOTIFICATION"; } }
//        public override String ShortInfo    { get { return "Modifying the default notification dispatcher."; } }
//        public override String Information  { get { return "Modifying the default notification dispatcher."; } }

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

//                _CommandGrammar.Root = NOTIFICATION;

//                return _CommandGrammar;

//            }

//        }


//        #endregion

//        #endregion

//        #region Constructor

//        public FSCLI_NOTIFICATION()
//        {

//            #region BNF rules

//            NOTIFICATION.Rule = NOTIFICATION_CommandString + NOTIFICATION_Dispatcher + NOTIFICATION_Type;
//            NOTIFICATION.PandoraOptions.Add(PandoraOption.IsCommandRoot);

//            NOTIFICATION_Dispatcher.Rule =      stringLiteral
//                                            |   DefaultSymbol;
//            NOTIFICATION_Dispatcher.PandoraOptions.Add(PandoraOption.IsOption);

//            NOTIFICATION_Type.Rule =        NOTIFICATION_BridgeSymbol + NOTIFICATION_Bridge_Options + CRSymbol
//                                        |   NOTIFICATION_DispatcherSymbol + NOTIFICATION_Dispatcher_Options + CRSymbol;
//            NOTIFICATION_Type.PandoraOptions.Add(PandoraOption.IsOption);
            
//            NOTIFICATION_Bridge_Options.Rule =      NOTIFICATION_StartSymbol
//                                                |   NOTIFICATION_StopSymbol
//                                                |   NOTIFICATION_StateSymbol;
//            NOTIFICATION_Bridge_Options.PandoraOptions.Add(PandoraOption.IsOption);

//            NOTIFICATION_Dispatcher_Options.Rule =      NOTIFICATION_SuspendSymbol
//                                                    |   NOTIFICATION_ResumeSymbol
//                                                    |   NOTIFICATION_StateSymbol;
//            NOTIFICATION_Dispatcher_Options.PandoraOptions.Add(PandoraOption.IsOption);


//            #endregion

//            #region Todo: via reflection

//            #region Non-terminal integration

//            _CommandNonTerminals.Add(NOTIFICATION);

//            #endregion

//            #region Symbol integration

//            _CommandSymbolTerminal.Add(NOTIFICATION_CommandString);
            
//            #endregion

//            #endregion

//        }

//        #endregion


//        #region Symbol declaration

//        SymbolTerminal NOTIFICATION_CommandString = Symbol("NOTIFICATION");
//        SymbolTerminal NOTIFICATION_BridgeSymbol = Symbol("bridge");
//        SymbolTerminal NOTIFICATION_DispatcherSymbol = Symbol("dispatcher");
//        SymbolTerminal NOTIFICATION_StartSymbol = Symbol("start");
//        SymbolTerminal NOTIFICATION_StopSymbol = Symbol("stop");
//        SymbolTerminal NOTIFICATION_SuspendSymbol = Symbol("suspend");
//        SymbolTerminal NOTIFICATION_ResumeSymbol = Symbol("resume");

//        SymbolTerminal NOTIFICATION_StateSymbol = Symbol("state");

//        #endregion

//        #region Non-terminal declaration

//        NonTerminal NOTIFICATION = new NonTerminal("NOTIFICATION");
//        NonTerminal NOTIFICATION_Dispatcher = new NonTerminal("NOTIFICATION_Dispatcher");
//        NonTerminal NOTIFICATION_Type = new NonTerminal("NOTIFICATION_Type");
//        NonTerminal NOTIFICATION_Bridge_Options = new NonTerminal("NOTIFICATION_Bridge_Options");
//        NonTerminal NOTIFICATION_Dispatcher_Options = new NonTerminal("NOTIFICATION_Dispatcher_Options");

//        #endregion

//        #region Terminal declaration



//        #endregion

//        #region Execute Command

//        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
//        {

//            NotificationDispatcher NotificationDispatcher = null;

//            #region Default NotificationDispatcher

//            if (myOptions.ElementAt(1).Value[0].Option.ToLower() == DefaultSymbol.Symbol.ToLower())
//            {

//                NotificationDispatcher = ((IGraphFSSession)myPFSObject).GetNotificationDispatcher();
//            }

//            #endregion

//            if (NotificationDispatcher == null)
//            {
//                WriteLine("No valid NotificationDispatcher found! Please use default or any other existing NotificationDispatcher.");
//                return;
//            }

//            #region NotificationBridge

//            if (myOptions.ElementAt(2).Value[0].Option.ToLower() == NOTIFICATION_BridgeSymbol.Symbol.ToLower())
//            {
//                if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_StartSymbol.Symbol.ToLower())
//                {
//                    NotificationDispatcher.StartBridge(((IGraphFSSession)myPFSObject).GetNotificationSettings().BridgePort);
//                }
//                else if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_StopSymbol.Symbol.ToLower())
//                {
//                    NotificationDispatcher.StopBridge();
//                }
//                else if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_StateSymbol.Symbol.ToLower())
//                {
//                    WriteLine("The NotificationBridge connection state is " + NotificationDispatcher.BridgeStarted);
//                }
//            }

//            #endregion

//            #region NotificationDispatcher

//            else if (myOptions.ElementAt(2).Value[0].Option.ToLower() == NOTIFICATION_DispatcherSymbol.Symbol.ToLower())
//            {
//                if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_SuspendSymbol.Symbol.ToLower())
//                {
//                    NotificationDispatcher.SuspendDispatcher();
//                }
//                else if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_ResumeSymbol.Symbol.ToLower())
//                {
//                    NotificationDispatcher.ResumeOrStartDispatcher();
//                }
//                else if (myOptions.ElementAt(3).Value[0].Option.ToLower() == NOTIFICATION_StateSymbol.Symbol.ToLower())
//                {
//                    WriteLine("The NotificationDispatcher running state is " + NotificationDispatcher.DispatcherRunning);
//                }

//            }

//            #endregion

//            //
//            //String _DirectoryObjectLocation = GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option);
//        }

//        #endregion

//    }

//}
