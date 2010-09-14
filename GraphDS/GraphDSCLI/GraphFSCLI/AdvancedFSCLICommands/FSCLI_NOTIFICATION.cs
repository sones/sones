///* GraphFS CLI - MKDIR
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
//using sones.Graph.Storage.GraphFS;
//using sones.Graph.Storage.GraphFS.Objects;
//using sones.Notifications;
//using sones.Lib.CLI
//using sones.Graph.Storage;
//using sones.Graph.Storage.GraphFS.Session;

//#endregion

//namespace sones.Graph.Applications.GraphFSCLI
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
//            NOTIFICATION.GraphOptions.Add(GraphOption.IsCommandRoot);

//            NOTIFICATION_Dispatcher.Rule =      stringLiteral
//                                            |   DefaultSymbol;
//            NOTIFICATION_Dispatcher.GraphOptions.Add(GraphOption.IsOption);

//            NOTIFICATION_Type.Rule =        NOTIFICATION_BridgeSymbol + NOTIFICATION_Bridge_Options + CRSymbol
//                                        |   NOTIFICATION_DispatcherSymbol + NOTIFICATION_Dispatcher_Options + CRSymbol;
//            NOTIFICATION_Type.GraphOptions.Add(GraphOption.IsOption);
            
//            NOTIFICATION_Bridge_Options.Rule =      NOTIFICATION_StartSymbol
//                                                |   NOTIFICATION_StopSymbol
//                                                |   NOTIFICATION_StateSymbol;
//            NOTIFICATION_Bridge_Options.GraphOptions.Add(GraphOption.IsOption);

//            NOTIFICATION_Dispatcher_Options.Rule =      NOTIFICATION_SuspendSymbol
//                                                    |   NOTIFICATION_ResumeSymbol
//                                                    |   NOTIFICATION_StateSymbol;
//            NOTIFICATION_Dispatcher_Options.GraphOptions.Add(GraphOption.IsOption);


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
