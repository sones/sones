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

/*
 * GraphCLI
 * Achim Friedland, 2008-2009
 * Henning Rauch, 2009
 */

#region Usings

using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;

using sones.Lib;
using sones.Lib.DDate;
using sones.Lib.DataStructures;
using sones.Lib.Frameworks.CLIrony.Compiler.Lalr;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDS.API.CSharp;
//using sones.Lib.Networking.Telnet;
//using sones.Lib.Networking.Telnet.Events;
//using sones.Lib.Networking.TCPSocket;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// The GraphDS CLI (command line interface)
    /// </summary>

    public class GraphCLI//: ATelnetServer
    {

        /// <summary>
        /// A grammar-based command line interface for all Graph
        /// subprojects.
        /// </summary>

        #region Data

        int                                     MaximumHistoryEntryLength = 1000;

        AGraphDSSharp                           _GraphDSSharp              = null;
        
        ConsoleColor                            default_color;

        Int32                                   bits                    = IntPtr.Size * 8;

        Regex                                   WhiteSpacePattern       = new Regex("[\\s]+");
        
        Grammar                                 GraphCLIGrammar       = new Grammar();
        LanguageCompiler                        GraphCLICompiler;
        CompilerContext                         _CompilerContext;
        Scanner                                 _Scanner;
        SourceFile                              _SourceFile;
        
        AbstractCLIOption                       CurrentOption           = null;

        List<String>                            NothingMountedList      = new List<String>();
        List<String>                            SthMountedList          = new List<String>();
        Int32                                   PositionInHistory       = 0;
        String                                  InputString             = "";
        Boolean                                 LoadStandardHistory     = true;
        String                                  HistoryFileName         = "";
        Dictionary<String, List<AbstractCLIOption>>.Enumerator      ParameterEnum;
        String                                  ActualHistoricCommand   = "";

        Dictionary<String, List<AbstractCLIOption>>    Parameters       = new Dictionary<String, List<AbstractCLIOption>>();
        Dictionary<String, AllCLICommands>        Commands;

        List<List<Token>>                       _TokenStream;
        List<ParserReturn>                      _CompletionList         = new List<ParserReturn>();

        String                                  CurrentPath             = "/";
        String                                  CurrentCommand          = "";

        Boolean                                 EnterPressed            = false;
        Boolean                                 ShutDownOnExit          = false;
        Boolean                                 IsQuit                  = false;

        //CLICommandCategory                      CommandCategory;
        int                                     currentCursorPosWithinCommand = 0;

        PerformanceCounter _CPUCounter;
        PerformanceCounter _RAMCounter;

        CLI_Output _CLI_Output;

        #region Telnet

        Boolean                                 IsTelnetCLI             = false;
        Queue<ConsoleKeyInfo>                   TelnetKeys              = null;

        HashSet<String>                         AutoCompletionTypeNames = null;
        Dictionary<String, AllCLIAutocompletions> AutoCompletions         = null;

        #endregion

        #region Console fields

        Int32                                   ConsoleWindowWidth;
        Int32                                   ConsoleWindowHeight;
        //Int32                                   ConsoleCursorTop;
        Int32                                   ConsoleCursorLeft;

        String                                  ConsolePrompt
        {
            get
            {
                return CurrentPath + " > ";
            }
        }

        #endregion

        #region External StreamWriter/StreamReader

        StreamWriter _StreamWriter;
        public StreamWriter StreamWriter
        {
            get { return _StreamWriter; }
            set { _StreamWriter = value; }
        }

        StreamReader _StreamReader;
        public StreamReader StreamReader
        {
            get { return _StreamReader; }
            set { _StreamReader = value; }
        }

        #endregion

        #endregion

        #region Constructors

        #region GraphCLI()

        /// <summary>
        /// This methods starts a GraphCLI without any predefined variables.
        /// Due to telnet usage, there will be NO StartupInformation printed
        /// </summary>
        public GraphCLI()
        {

            LoadGrammar();
            //PrintStartupInformation();
            initConsole();
            initCli();

            ShutDownOnExit = true;
        }

        #endregion

        #region GraphCLI(myCommandTypes)

        /// <summary>
        /// This methods starts a GraphCLI loading just the requested
        /// command types.
        /// </summary>
        public GraphCLI(params Type[] myCommandTypes)
        {
            ShutDownOnExit = false;

            initCli(myCommandTypes);
        }

        #endregion

        #region GraphCLI(myGraphDSSharp, myPath)

        /// <summary>
        /// This methods starts a GraphCLI using the given GraphFS and myPath
        /// </summary>
        /// <param name="myGraphDSSharp">an already defined virtual file system</param>
        /// <param name="myPath">the location/myPath within the file system</param>
        public GraphCLI(AGraphDSSharp myGraphDSSharp, String myPath)
            : this()
        {

            _GraphDSSharp  = myGraphDSSharp;
            CurrentPath = myPath;
            ShutDownOnExit = false;
            PrintStartupInformation();

        }

        #endregion

        #region GraphCLI(myGraphDSSharp, myPath, params Type[] myCommandTypes)

        /// <summary>
        /// This methods starts a GraphCLI using the given GraphFS and
        /// myPath loading just the requested command types.
        /// </summary>
        /// <param name="myGraphDSSharp">an already defined virtual file system</param>
        /// <param name="myPath">the location/myPath within the file system</param>
        public GraphCLI(AGraphDSSharp myGraphDSSharp, String myPath, params Type[] myCommandTypes) 
        {

            _GraphDSSharp  = myGraphDSSharp;
            CurrentPath = myPath;

            initCli(myCommandTypes);

        }

        #endregion

        #region GraphCLI(myGraphDSSharp, myDatabasePath, myStream, myCLI_Output, params myCommandTypes)

        public GraphCLI(AGraphDSSharp myGraphDSSharp, String myDatabasePath, Stream myStream, CLI_Output myCLI_Output, params Type[] myCommandTypes)
        {
            _GraphDSSharp = myGraphDSSharp;
            CurrentPath = myDatabasePath;
            _StreamWriter = new StreamWriter(myStream);
            _StreamReader = new StreamReader(myStream);
            _CLI_Output = myCLI_Output;

            initCli(myCommandTypes);
        }

        #endregion

        #endregion


        #region Init

        /// <summary>
        /// If we are in tests there is no Console.WindowHeight etc available...
        /// </summary>
        private void initConsole()
        {
            try
            {
                ConsoleWindowHeight = Console.WindowHeight;
                ConsoleWindowWidth = Console.WindowWidth;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            }
            catch (Exception ex)
            {
                //NLOG: temporarily commented
                //_Logger.ErrorException("Console not available ", ex);
            }
        }

        private void initCli(params Type[] myCommandTypes)
        {

            if (_CLI_Output == CLI_Output.Standard)
                PrintStartupInformation();
            
            LoadGrammar(myCommandTypes);
            
            if (!IsTelnetCLI && _StreamWriter == null)
            {
                initConsole();
            }

            _CPUCounter = new PerformanceCounter();

#if __MonoCS__
            _CPUCounter.CategoryName = "Process";
            _CPUCounter.CounterName = "% Processor Time";
            _CPUCounter.InstanceName = Process.GetCurrentProcess().Id.ToString();

            _RAMCounter = new PerformanceCounter(); 
            _RAMCounter.CategoryName = "Process";
            _RAMCounter.CounterName = "Working Set";
            _RAMCounter.InstanceName = Process.GetCurrentProcess().Id.ToString();

#else
            _CPUCounter.CategoryName = "Process";
            _CPUCounter.CounterName = "% Processor Time";
            _CPUCounter.InstanceName = Process.GetCurrentProcess().ProcessName;

            _RAMCounter = new PerformanceCounter(); 
            _RAMCounter.CategoryName = "Process";
            _RAMCounter.CounterName = "Working Set - Private";
            _RAMCounter.InstanceName = Process.GetCurrentProcess().ProcessName;

#endif


        }

        #endregion

        #region Console_CancelKeyPress Strg+C

        void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (Commands.ContainsKey(CurrentCommand))
            {
                e.Cancel = Commands[CurrentCommand].CancelCurrentCommand(sender, e);
            }
        }

        #endregion


        #region (private) Startup methods

        #region (private) LoadGrammar()

        private void LoadGrammar()
        {
            LoadGrammar(typeof(AllCLICommands));
        }

        #endregion

        #region (private) LoadGrammar(myCommandTypes)

        private void LoadGrammar(params Type[] myCommandTypes)
        {
            FindAndRegisterCLICommands(myCommandTypes);
            BuildGraphCLIGrammar();
            GraphCLICompiler = new LanguageCompiler(GraphCLIGrammar);

            #region Load Autocompletion

            AutoCompletionTypeNames = new HashSet<string>();
            foreach (Terminal aTerminal in GraphCLIGrammar.Terminals)
            {
                if (aTerminal.GraphOptions.Contains(GraphOption.IsUsedForAutocompletion))
                {
                    AutoCompletionTypeNames.Add(aTerminal.Name);
                }
            }

            LoadAutoCompletion();

            #region Error Handling

            foreach (String aAcName in AutoCompletionTypeNames)
            {
                if (!AutoCompletions.ContainsKey(aAcName.ToLower()))
                {
                    if (_CLI_Output == CLI_Output.Standard)
                        WriteLine("WARNING! \"" + aAcName + "\"" + " Autocompletion is not available.");
                }
            }

            #endregion

            #endregion
        
        }

        #endregion

        #region (private) LoadAutoCompletion()

        private void LoadAutoCompletion()
        {
            AutoCompletions = new Dictionary<string, AllCLIAutocompletions>();

            foreach (string fileOn in Directory.GetFiles("."))
            {

                FileInfo file = new FileInfo(fileOn);

                // Preliminary check, must be .dll or .exe
                if (file.Extension.Equals(".dll") || file.Extension.Equals(".exe"))
                {

                    try
                    {

                        Type[] _AllTypes = Assembly.LoadFrom(file.FullName).GetTypes();

                        foreach (Type _ActualType in _AllTypes)
                        {
                            Boolean _AddAutoCompletion = false;

                            foreach (String aAutoCompletionName in AutoCompletionTypeNames)
                            {
                                if (_ActualType.Name.ToUpper().Equals(aAutoCompletionName.ToUpper()))
                                {
                                    _AddAutoCompletion = true;
                                    break;
                                }
                            }

                            if (_AddAutoCompletion)
                            {
                                AllCLIAutocompletions _AGraphCLIAutocompletion = (AllCLIAutocompletions)Activator.CreateInstance(_ActualType);

                                if (AutoCompletions.ContainsKey(_AGraphCLIAutocompletion.Name))
                                    WriteLine("Duplicate autocompletion: " + _ActualType.Name);

                                else
                                {
                                    AutoCompletions.Add(_AGraphCLIAutocompletion.Name.ToLower(), _AGraphCLIAutocompletion);
                                }
                            }
                        }//
                    }
                    catch (Exception e)
                    {
                        WriteLine(file.Name + " failed. " + e.Message);
                    }
                }//dll||exe?
            }//foreach file in directory
        }

        #endregion

        #region (private) FindAndRegisterCLICommands()

        private void FindAndRegisterCLICommands()
        {
            FindAndRegisterCLICommands(typeof(AllCLICommands));
        }

        #endregion

        #region (private) FindAndRegisterCLICommands(myCommandTypes)

        private void FindAndRegisterCLICommands(params Type[] myCommandTypes)
        {

            Commands = new Dictionary<String, AllCLICommands>();
         
            foreach (string fileOn in Directory.GetFiles("."))
            {

                FileInfo file = new FileInfo(fileOn);

                // Preliminary check, must be .dll or .exe
                if (file.Extension.Equals(".dll") || file.Extension.Equals(".exe"))
                {

                    try
                    {

                        var _AllTypes = Assembly.LoadFrom(file.FullName).GetTypes();

                        foreach (Type _ActualType in _AllTypes)
                        {

                            var _AddCommand = false;

                            if (_ActualType.BaseType != null)
                                if (_ActualType.BaseType == typeof(AAllInBuildCLICommands))
                                    _AddCommand = true;

                                else
                                    foreach (Type _CommandType1 in myCommandTypes)
                                        if (_ActualType.BaseType == _CommandType1)
                                            _AddCommand = true;

                                        else if (_ActualType.BaseType.BaseType != null)
                                            foreach (Type _CommandType2 in myCommandTypes)
                                                if (_ActualType.BaseType.BaseType == _CommandType2)
                                                    _AddCommand = true;

                                                else if (_ActualType.BaseType.BaseType.BaseType != null)
                                                    foreach (Type _CommandType3 in myCommandTypes)
                                                        if (_ActualType.BaseType.BaseType.BaseType == _CommandType3)
                                                            _AddCommand = true;

                                                        else if (_ActualType.BaseType.BaseType.BaseType.BaseType != null)
                                                            foreach (Type _CommandType4 in myCommandTypes)
                                                                if (_ActualType.BaseType.BaseType.BaseType.BaseType == _CommandType4)
                                                                    _AddCommand = true;

                                                                else if (_ActualType.BaseType.BaseType.BaseType.BaseType.BaseType != null)
                                                                    foreach (Type _CommandType5 in myCommandTypes)
                                                                        if (_ActualType.BaseType.BaseType.BaseType.BaseType.BaseType == _CommandType5)
                                                                            _AddCommand = true;
                            
   
                            if (_AddCommand)
                            {

                                // Do not try to instantiate abstract classes!
                                if ((_ActualType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
                                {

                                    var _AGraphCLICommand = (AllCLICommands) Activator.CreateInstance(_ActualType);
                                    _AGraphCLICommand.SetCLIReference(this);
                                    
                                    _AGraphCLICommand.CLI_Output = _CLI_Output;

                                    try
                                    {
                                        //lets see, if there is a valid grammar
                                        Object o = _AGraphCLICommand.CommandGrammar;

                                        if (Commands.ContainsKey(_AGraphCLICommand.Command))
                                            WriteLine("Duplicate command: " + _ActualType.Name);

                                        else
                                        {
                                            //                                        WriteLine("Found: " + _ActualType.Name);
                                            Commands.Add(_AGraphCLICommand.Command, _AGraphCLICommand);
                                        }
                                    }
                                    catch (NotImplementedException e)
                                    {
                                        WriteLine("ERROR! The command \"{0}\" has not been specified as a BNF grammar. It will not be integrated into the GraphCLI! " + e.Message, _AGraphCLICommand.Command);
                                    }  
                                }
                            }

                        }//foreach

                    }
                    catch (Exception e)
                    {
                        WriteLine(file.Name + " failed. " + e.Message);
                    }

                }

            }

            //WriteLine("");

        }

        #endregion

        #region (private) BuildGraphCLIGrammar()

        private void BuildGraphCLIGrammar()
        {

            #region global grammar options

            GraphCLIGrammar.CaseSensitive = false;

            NonTerminal _Command = new NonTerminal("Command");

            List<ABnfTerm> _SubtreeCommandGrammarRoot = new List<ABnfTerm>();

            #endregion

            #region get command specific grammars

            foreach (KeyValuePair<String, AllCLICommands> _aCommand in Commands)
            {
                //TODO: check if the grammar is valid. if not --> handle error
                Grammar _aCommandGrammar = _aCommand.Value.CommandGrammar;
                LanguageCompiler _testCompiler = new LanguageCompiler(_aCommandGrammar);
                Boolean isGrammarValid = true;

                foreach (String aErrorString in _testCompiler.Grammar.Errors)
                {
                    if (aErrorString.StartsWith("Error"))
                    {
                        WriteLine(aErrorString);
                        isGrammarValid = false;
                    }
                }

                if (isGrammarValid)
                {

                    GraphCLIGrammar.NonTerminals.AddRange(_aCommandGrammar.NonTerminals);

                    GraphCLIGrammar.Terminals.AddRange(_aCommandGrammar.Terminals);

                    _SubtreeCommandGrammarRoot.Add(_aCommandGrammar.Root);
                }
                else
                {
                    WriteLine("ERROR! The command \"{0}\" is defined by an incorrect BNF grammar. It is not integrated into the GraphCLI!");

                }
            }

            #endregion

            #region build new root

            for (int i = 0; i < _SubtreeCommandGrammarRoot.Count; i++)
            {
                BnfExpression _exprNew = new BnfExpression(_SubtreeCommandGrammarRoot[i]);

                if (!i.Equals(0))
                {
                    BnfExpression _exprActual = new BnfExpression(_Command.Rule);
                    
                    _Command.Rule = _exprActual | _exprNew;
                }
                else
                {
                    _Command.Rule = _exprNew;
                }
            }
 
            GraphCLIGrammar.NonTerminals.Add(_Command);

            GraphCLIGrammar.Root = _Command;

            #endregion

        }

        #endregion

        #region (private) PrintStartupInformation()

        private void PrintStartupInformation()
        {

            if (!IsTelnetCLI)
            {
                //Console.Title = "sones GraphCLI v" + CLIVersion.VersionString;
                Console.ForegroundColor = ConsoleColor.White;
            }

            WriteLine("sones GraphDS v{0} - {1}bit mode", CLIVersion.VersionString, bits);
            WriteLine("Copyright 2007-2010 sones GmbH - http://www.sones.com");
            WriteLine();
            WriteLine(new DiscordianDate().ToString());
            WriteLine();
            WriteLine("Use '?' for help and 'quit' to quit. Thank you.");

        }

        #endregion

        #endregion


        #region (private) Helper methods

        #region (private) WritePromptToConsole(default_color, CurrentPath)

        private void WritePromptToConsole(ConsoleColor default_color, String CurrentPath)
        {
            if (!IsTelnetCLI)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                // Current Path
                Console.Write(CurrentPath);
                Console.ForegroundColor = ConsoleColor.Green;
                // the shell
                Console.Write(" > ");
                Console.ForegroundColor = default_color;
            }
            else
            {
                Write(ConsolePrompt);
                ConsoleCursorLeft += ConsolePrompt.Length;
            }
        }

        #endregion

        #region (private) HandleHistoricCommand(InputString, AHistoricalCommand)

        private void HandleHistoricCommand(String InputString, String AHistoricalCommand)
        {
            if (InputString.Length > AHistoricalCommand.Length)
            {
                for (int i = 0; i < InputString.Length; i++)
                {
                    Write(" ");
                }
                SetCursorPosition(Console.CursorLeft - (InputString.Length - AHistoricalCommand.Length + 2), Console.CursorTop);
            }
            else
            {
                //SetCursorPosition(ConsoleCursorLeft - (InputString.Length - AHistoricalCommand.Length + 2), ConsoleCursorTop);
            }
        }

        #endregion

        #region (private) PrintFilteredCommands(ToBePrinted)

        private void PrintFilteredCommands(List<ParserReturn> ToBePrinted)
        {
            WriteLine(Environment.NewLine);
            //ToBePrinted.Sort();

            foreach (ParserReturn aCommand in ToBePrinted)
            {
                if (aCommand.isUsedForAutoCompletion)
                {
                    WriteLine("  " + aCommand.description);
                }
                else
                {
                    WriteLine("  " + aCommand.name);
                }
            }

            WriteLine(Environment.NewLine);
        }

        #endregion

        #region (private) AddOptionToParameters(aCLIoption)

        private void AddOptionToParameters(AbstractCLIOption aCLIoption)
        {
            CurrentOption = aCLIoption;

            if (!Parameters.ContainsKey(aCLIoption.Option))
            {
                List<AbstractCLIOption> NewParameterList = new List<AbstractCLIOption>();
                NewParameterList.Add(aCLIoption);
                Parameters.Add(aCLIoption.Option, NewParameterList);
            }//new Option?
            else
            {
                Parameters[aCLIoption.Option].Add(aCLIoption);
            }//no new Option
        }

        #endregion

        #region (private) HandleBackSpace(NumberOfCharsPre, NumberOfCharsPost)

        private void HandleBackSpace(int NumberOfCharsPre, int NumberOfCharsPost)
        {
            //bytesToSend = new Byte[1] { b }; 
            if (InputString.Length >= 1)
            {
                InputString = InputString.Remove(InputString.Length - 1);

                if (IsTelnetCLI)
                {
                    ////TcpClientConnection.GetStream().
                    //TelnetWrite(new Byte[] { 27, 91, 68, 32, 27, 91, 68 }, 0, 7);
                    //ConsoleCursorLeft--;
                }
                else
                {
                    if (NumberOfCharsPre.Equals(NumberOfCharsPost))
                    {
                        SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop - 1);
                        Write(" ");
                        SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop - 1);
                    }
                    else
                    {
                        Write(" ");
                        SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
            }//InputString.Length >= 1 ?
            else
            {
                if (!IsTelnetCLI)
                    SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                //Write(" ");
            }//else

        }

        #endregion

        //#region (private) GetCurrentCommandCategory(CurrentCommand)

        //private CLICommandCategory GetCurrentCommandCategory(string CurrentCommand)
        //{
        //    byte CommandSign;
        //    CommandSign = (byte)Commands[CurrentCommand].aCategory;

        //    if ((CommandSign & (byte)CLICommandCategory.FSCommand).CompareTo(1) >=0)
        //        return CLICommandCategory.FSCommand;
        //    else
        //        if ((CommandSign & (byte)CLICommandCategory.DBCommand).CompareTo(1) >= 0)
        //            return CLICommandCategory.DBCommand;
        //        else
        //            return CLICommandCategory.CLIStandardCommand;
        //}//method

        //#endregion

        #region (private) ExtractOptionsFromTree(_ExecutionTree)

        private void ExtractOptionsFromTree(AstNode _ExecutionTree)
        {

            List<AstNode> AllNodes = (List<AstNode>)_ExecutionTree.GetAll();

            //get the command
            CurrentCommand = AllNodes[0].GetContent().Clone().ToString();

            //get the category of the command
            //CommandCategory = GetCurrentCommandCategory(CurrentCommand);

            List<AstNode> ToBeDeleted = new List<AstNode>();

            foreach (AstNode aAstNOde in AllNodes)
            {
                if (aAstNOde.Term.GraphOptions.Contains(GraphOption.IsStructuralObject) || aAstNOde.Term.GraphOptions.Contains(GraphOption.IsCommandRoot))
                {
                    ToBeDeleted.Add(aAstNOde);
                }
            }

            foreach (AstNode aElement in ToBeDeleted)
            {
                AllNodes.Remove(aElement);
            }

            //finally, remove first Element (the command)
            AddOptionToParameters(CreateNewOption(AllNodes[0].GetContent(), AllNodes[0].Span));
            AllNodes.RemoveAt(0);

            bool _IsNewOption = false;
            bool _DidIEverDetectAnOption = false;
            foreach (AstNode aAstNOde in AllNodes)
            {
                if (aAstNOde.Term.GraphOptions.Contains(GraphOption.IsOption))
                {
                    //wohoo, new Option
                    _IsNewOption = true;
                    _DidIEverDetectAnOption = true;
                }
                else
                {
                    if (_IsNewOption)
                    {
                        AddOptionToParameters(CreateNewOption(aAstNOde.GetContent(), aAstNOde.Span));

                        _IsNewOption = false;
                    }
                    else
                    {
                        if (!_DidIEverDetectAnOption)
                        {
                            //for one-line bnf-rules
                            AddOptionToParameters(CreateNewOption(aAstNOde.GetContent(), aAstNOde.Span));
                        }
                        else
                        {
                            int _ActualCountOfOptions = Parameters[CurrentOption.Option].Count;
                            Parameters[CurrentOption.Option][_ActualCountOfOptions - 1].AddParameter(aAstNOde.GetContent());
                        }
                    }


                }
            }
        }

        #endregion

        #region (private) AbstractCLIOption CreateNewOption(_Name, _Span)

        private AbstractCLIOption CreateNewOption(string _Name, SourceSpan _Span)
        {
            AbstractCLIOption _ACLIOption = new AbstractCLIOption(_Name);
            _ACLIOption.Column = _Span.Start.Column;
            _ACLIOption.EndPos = _Span.EndPos;
            _ACLIOption.Line = _Span.Start.Line;
            _ACLIOption.Pos = _Span.Start.Position;

            return _ACLIOption;
        }

        #endregion

        #region (private) Reset()

        private void Reset()
        {
            Parameters.Clear();
            CurrentCommand = "";
            CurrentOption = null;
            InputString = "";
            ConsoleCursorLeft = 0;
            currentCursorPosWithinCommand = 0;
        }

        #endregion

        #region (private) ValidCommandFromInputString(InputString)

        private bool ValidCommandFromInputString(string InputString)
        {
            if (InputString.Length.Equals(0))
            {
                return false;
            }
            else
            {
                String ToBeValidatedInputString = InputString.TrimStart().Split(' ')[0].ToUpper();
                if (Commands.ContainsKey(ToBeValidatedInputString))
                {
                    return true;
                }
                else
                {
                    WriteLastInputstringToConsole(InputString);
                    WriteLine(Environment.NewLine + "No such command as {0}", ToBeValidatedInputString);
                    Reset();
                    return false;
                }
            }
        }

        #endregion

        #region (private) MarkWrongOption(InputString, WrongOption)

        private void MarkWrongOption(string InputString, List<String> CorrectParts)
        {
            String TempInputString = InputString.Clone().ToString();

            foreach (String aCorrectString in CorrectParts)
            {
                TempInputString = TempInputString.ToUpper().Replace(aCorrectString.ToUpper(), "");
            }

            TempInputString = TempInputString.Trim();

            if (TempInputString.Length.Equals(0))
            {
                if (!IsTelnetCLI)
                    WriteLastInputstringToConsole(InputString);

                WriteLine(Environment.NewLine + "The following command is incomplete: {0}", InputString.TrimStart());
                Reset();
            }
            else
            {
                int BeginOfRedArea = InputString.ToUpper().LastIndexOf(TempInputString);

                String RightPart = InputString.Substring(0, BeginOfRedArea);
                String WrongPart = InputString.Substring(BeginOfRedArea);

                if (!IsTelnetCLI)
                    WriteLastInputstringToConsole(InputString);

                WriteLine(Environment.NewLine + "The command contains errors somewhere in the red marked area:");
                Write(RightPart.TrimStart());
                if (!IsTelnetCLI)
                    Console.ForegroundColor = ConsoleColor.Red;
                Write(WrongPart + Environment.NewLine);
                if (!IsTelnetCLI)
                    Console.ForegroundColor = default_color;
                Reset();
            }
        }

        #endregion

        #region (private) WriteLastInputstringToConsole(InputString)

        private void WriteLastInputstringToConsole(string _InputString)
        {
            WritePromptToConsole(default_color, CurrentPath);
            Write(_InputString);
        }

        #endregion

        #region (private) LoadHistoryFrom(HistoryFileName)

        private void LoadHistoryFrom(string HistoryFileName)
        {

            if (File.Exists(HistoryFileName))
            {
                StreamReader myFile = new StreamReader(HistoryFileName, System.Text.Encoding.Default);
                while (!myFile.EndOfStream)
                {
                    SthMountedList.Add(myFile.ReadLine());
                }
                myFile.Close();
            }

        }

        #endregion

        #region SaveHistory(HistoryFileName)

        private void SaveHistory(string HistoryFileName, List<String> HistoryCommands)
        {
            StreamWriter mySaveFile = new StreamWriter(HistoryFileName, false);

            foreach (String aHistoryCommand in HistoryCommands)
            {
                mySaveFile.WriteLine(aHistoryCommand);
            }

            mySaveFile.Close();
        }

        #endregion

        #region (private) ResetInputLine()

        private void ResetInputLineAndString()
        {
            ResetInputLine();
            InputString = "";
        }

        private void ResetInputLine()
        {

            if (IsTelnetCLI)
            {
                //List<Byte> bytesToSend = new List<byte>();
                //for (int i = 0; i < InputString.Length; i++)
                //    bytesToSend.AddRange(new Byte[] { 27, 91, 68, 32, 27, 91, 68 });

                //TelnetWrite(bytesToSend.ToArray(), 0, bytesToSend.Count);
                //ConsoleCursorLeft -= InputString.Length;

            }
            else
            {
                try
                {
                    Write(" ");
                    int NumberOfLines = ((InputString.Length + CurrentPath.Length + 3) / Console.WindowWidth) + 1;

                    SetCursorPosition(CurrentPath.Length + 3, Console.CursorTop - NumberOfLines + 1);
                    for (int i = 0; i < InputString.Length + 1; i++)
                        Write(" ");

                    SetCursorPosition(CurrentPath.Length + 3, Console.CursorTop - NumberOfLines + 1);
                }
                catch (Exception)
                { }
            }
        }
        #endregion

        #region (private) UseGraphHistory (HistoryList)

        private void UseGraphHistory(ref List<String> HistoryList, ConsoleKeyInfo? myInput)
        {
            #region Data

            int CountHistoricalEntries = HistoryList.Count;

            #endregion

            if (!CountHistoricalEntries.Equals(0))
            {
                #region Data

                switch (myInput.Value.Key)
                {
                    case ConsoleKey.UpArrow:

                        if (PositionInHistory.CompareTo(CountHistoricalEntries - 1) <= 0)
                        {
                            //get the current history entry
                            ActualHistoricCommand = HistoryList[CountHistoricalEntries - PositionInHistory - 1];

                            //reset the InputLine
                            ResetInputLineAndString();

                            //print actutal entry in history
                            Write(ActualHistoricCommand);

                            //update InputString
                            InputString = ActualHistoricCommand.Clone().ToString();

                            //update PositionInHistory
                            PositionInHistory++;

                        }//is there a command left?
                        else
                        {
                            //leave InputLine as it is --> there should be a command already
                            //reset the InputLine
                            ResetInputLineAndString();

                            //print actutal entry in history
                            Write(ActualHistoricCommand);

                            //update InputString
                            InputString = ActualHistoricCommand.Clone().ToString();
                        }

                        break;
                    case ConsoleKey.DownArrow:

                        if ((PositionInHistory - 1).CompareTo(0) > 0)
                        {
                            //get the previous history entry
                            ActualHistoricCommand = HistoryList[CountHistoricalEntries - PositionInHistory + 1];

                            //reset the InputLine
                            ResetInputLineAndString();

                            //print actutal entry in history
                            Write(ActualHistoricCommand);

                            //update InputString
                            InputString = ActualHistoricCommand.Clone().ToString();

                            //update PositionInHistory
                            PositionInHistory--;
                        }//is there a command left?
                        else
                        {
                            //leave InputLine as it is --> there should be a command already
                            //reset the InputLine
                            ResetInputLineAndString();

                            //print actutal entry in history
                            Write(ActualHistoricCommand);

                            //update InputString
                            InputString = ActualHistoricCommand.Clone().ToString();
                        }

                        break;

                }

                #endregion

                currentCursorPosWithinCommand = InputString.Length;
                
            }//more than 1 history entry?
        }

        #endregion

        #region (private) FindMatchingPrefix(, prefix, matchings)

        private string FindMatchingPostfix(string prefix, List<string> matchings)
        {
            #region Data

            String equalElements = "";
            int index = 0;
            Boolean stop = false;

            #endregion

            if (matchings.Count.CompareTo(0) > 0)
            {
                foreach (char aChar in matchings[0])
                {
                    foreach (string aString in matchings)
                    {
                        if (aString.Length.CompareTo(index) > 0)
                        {

                            if (!aString[index].Equals(aChar))
                            {
                                stop = true;
                                break;
                            }
                        }
                        else
                        {
                            stop = true;
                            break;
                        }
                    }

                    if (!stop)
                    {
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (index.CompareTo(0) > 0)
                {
                    equalElements = matchings[0].Substring(prefix.Length, index - prefix.Length);
                }
            }
            return equalElements;
        }

        #endregion

        #region (private) HandleAcOutput(AcLines, ParserReturn)

        private void HandleAcOutput(List<AcInformation> AcInformationList)
        {
            #region Data

            Dictionary<int, List<String>> MatchingDict = new Dictionary<int,List<string>>();
            Boolean stop = false;
            List<String> ListOfMatchingPostfixes = new List<string>();

            #endregion

            
            if (AcInformationList != null)
            {
                #region Calc Matchings
                int counter = 0;
                foreach (AcInformation aAcInfo in AcInformationList)
                {
                    List<String> localMatching = new List<string>();

                    foreach (String aAcLine in aAcInfo.AcLines)
                    {
                        if (aAcLine.StartsWith(aAcInfo.ToBeAced))
                        {
                            localMatching.Add(aAcLine);
                        }
                    }

                    if (localMatching.Count.CompareTo(0) > 0)
                    {
                        MatchingDict.Add(counter, localMatching);
                    }
                    counter++;
                }

                #endregion

                #region checking, if there's an Ac with one hit

                foreach (KeyValuePair<int, List<String>> aMatching in MatchingDict)
                {
                    //just take the first hit... nothing else
                    if (aMatching.Value.Count.Equals(1))
                    {
                        WriteLine(Environment.NewLine);

                        String acResult = aMatching.Value[0];

                        WriteLine("  " + acResult);

                        WriteLine(Environment.NewLine);
                        WritePromptToConsole(default_color, CurrentPath);

                        String PreInputString = InputString.Substring(0, AcInformationList[aMatching.Key].IndexOfLastDelimiter + 1);
                        String PostInputString = InputString.Substring(AcInformationList[aMatching.Key].IndexOfLastDelimiter + 1 + AcInformationList[aMatching.Key].ToBeAced.Length);

                        InputString = PreInputString + acResult + PostInputString;

                        Write(InputString);

                        currentCursorPosWithinCommand = currentCursorPosWithinCommand + (acResult.Length - AcInformationList[aMatching.Key].ToBeAced.Length);

                        stop = true;
                    }
                }

                #endregion

                if (!stop)
                {
                    foreach (KeyValuePair<int, List<String>> aMatching in MatchingDict)
                    {
                        #region find equal prefixes

                        ListOfMatchingPostfixes.Add(FindMatchingPostfix(AcInformationList[aMatching.Key].ToBeAced, aMatching.Value));

                        #endregion

                        #region print

                        WriteLine(Environment.NewLine);
                        WriteLine(AcInformationList[aMatching.Key].AcMetadata.description);

                        foreach (String aLine in aMatching.Value)
                        {
                            WriteLine("  " + aLine);
                        }

                        WriteLine(Environment.NewLine);

                        #endregion
                    }

                    #region find shortest postfix

                    int index = 0;
                    int lengthOfShortest = Int32.MaxValue;
                    int bestIndex = -1;
                    foreach (String aMatchingPostfix in ListOfMatchingPostfixes)
                    {
                        if (aMatchingPostfix.Length.CompareTo(lengthOfShortest) < 0)
                        {
                            lengthOfShortest = aMatchingPostfix.Length;
                            bestIndex = index;
                        }

                        index++;
                    }

                    #endregion

                    WritePromptToConsole(default_color, CurrentPath);

                    // by ahzf:
                    if (bestIndex >= 0)
                        InputString = String.Concat(InputString, ListOfMatchingPostfixes[bestIndex]);

                    Write(InputString);

                    // by ahzf:
                    if (bestIndex >= 0)
                        currentCursorPosWithinCommand = currentCursorPosWithinCommand + ListOfMatchingPostfixes[bestIndex].Length;

                }

            }

        }

        #endregion

        #region (private) UseAutocompletion()

        private void UseAutocompletion()
        {
            #region Data

            int index = InputString.Substring(0, currentCursorPosWithinCommand).LastIndexOf("'");
            if (index < 0) index = 0;
            String prefrix = InputString.Substring(0, index);
            List<ParserReturn> tempReturn = GetPossibleTokensViaIrony(prefrix);
            List<AcInformation> AcInformationList = new List<AcInformation>();

            #endregion

            if (tempReturn.Count.Equals(1))
            {
                AcInformation aSingleAcInformation = UseAutocompletion(tempReturn[0]);

                if (aSingleAcInformation != null)
                {
                    AcInformationList.Add(aSingleAcInformation);
                }

            }
            else
            {
                //find the right one
                foreach (ParserReturn aParserReturn in tempReturn)
                {
                    if (aParserReturn.isUsedForAutoCompletion)
                    {
                        AcInformation aAcInformation = UseAutocompletion(aParserReturn);

                        if (aAcInformation != null)
                        {
                            AcInformationList.Add(aAcInformation);
                        }
                    }
                }
            }

            if (!AcInformationList.Count.Equals(0))
            {
                HandleAcOutput(AcInformationList);
            }
        }

        #endregion

        #region (private) UseAutocompletion(AcClass)

        private AcInformation UseAutocompletion(ParserReturn AcMetadata)
        {
            #region Data

            String toBeACed = "";
            String InputStringSubstringCursorPrefix = InputString.Substring(0, currentCursorPosWithinCommand);

            #endregion

            //get the already typed Literal, that has to be autocompleted
            int lastIndexOfDelimitter = InputStringSubstringCursorPrefix.LastIndexOf('\'');
            toBeACed = InputStringSubstringCursorPrefix.Substring(lastIndexOfDelimitter).Replace("'", "");

            if (AutoCompletions.ContainsKey(AcMetadata.name.ToLower()))
            {
                return new AcInformation(AutoCompletions[AcMetadata.name.ToLower()].Complete(_GraphDSSharp, ref CurrentPath, toBeACed), AcMetadata, toBeACed, lastIndexOfDelimitter);

            }//is this AC available?
            else
            {
                WriteLine(Environment.NewLine);

                WriteLine("Warning! Sorry, but the \"{0}\" is currently not available!{1}Please type this one manually.", AcMetadata.name, Environment.NewLine);

                WriteLine(Environment.NewLine);

                WritePromptToConsole(default_color, CurrentPath);

                Write(InputString);

                return null;
            }
        }

        #endregion

        #region (private) List<ParserReturn> GetPossibleTokensViaIrony(Input)

        private List<ParserReturn> GetPossibleTokensViaIrony(String Input)
        {
            List<ParserReturn> tempCompletionList = new List<ParserReturn>();

            #region set up of autocompletion environment

            _Scanner = GraphCLICompiler.Scanner;

            _CompilerContext = new CompilerContext(GraphCLICompiler);

            #endregion

            #region get possible tokens

            _SourceFile = new SourceFile(Input, "Source");

            _Scanner.Prepare(_CompilerContext, _SourceFile);

            _CompilerContext.Tokens.Clear();

            _TokenStream = _Scanner.BeginNonDetermisticScan();

            tempCompletionList = GraphCLICompiler.Parser.GetPossibleTokens(_CompilerContext, _TokenStream, Input);

            #endregion

            return tempCompletionList;
        }

        #endregion

        #region (private) Boolean AreWeInAnAutocompletionState()

        private Boolean AreWeInAnAutocompletionState()
        {
            #region Data

            // by ahzf:
            if (currentCursorPosWithinCommand > InputString.Length)
                currentCursorPosWithinCommand = InputString.Length;

            String InputStringSubstringCursorPrefix = InputString.Substring(0, currentCursorPosWithinCommand);
            int countOfDelimittersInSubstring = 0;

            #endregion

            //calculating the count of delimmiters in InputStringPrefix
            //if there is an even count we cannot be inside of an extended AC
            foreach (char aChar in InputStringSubstringCursorPrefix)
            {
                if (aChar.Equals('\''))
                {
                    countOfDelimittersInSubstring++;
                }
            }

            if ((countOfDelimittersInSubstring % 2).Equals(0))
            {
                return false;
            }//even?
            else
            {
                //odd!
                //so, there has to be some AC done

                return true;
            }
        }

        #endregion

        #region (private) GetConsoleKeyInfo()

        private ConsoleKeyInfo? GetConsoleKeyInfo()
        {
            if (IsTelnetCLI)
            {

                ConsoleKeyInfo keyInfo;

                Monitor.Enter(TelnetKeys);

                    if (TelnetKeys.Count > 0)
                        return TelnetKeys.Dequeue();

                Monitor.Wait(TelnetKeys);

                Monitor.Exit(TelnetKeys);

                lock (TelnetKeys)
                {

                    if (TelnetKeys.Count == 0)
                        return null;

                    keyInfo = TelnetKeys.Dequeue();
                    
                    return keyInfo;

                }

            }
            else if (_StreamReader != null)
            {
                var onechar = new Char[1];
                _StreamReader.Read(onechar, 0, 1);
                byte b = (byte)onechar[0];

                ConsoleKeyInfo? keyInfo = null;

                // numbers
                if ((b >= 48 && b <= 57)
                    // char a-z
                    || (b >= 65 && b <= 90))
                {
                    keyInfo = new ConsoleKeyInfo(onechar[0], (ConsoleKey)b, false, false, false);
                }
                // char A-Z
                else if (b >= 97 && b <= 122)
                {
                    keyInfo = new ConsoleKeyInfo(onechar[0], (ConsoleKey)(b - 32), false, false, false);
                }
                // Enter
                else if (b == 0x0D)// (Byte)ASCIIControlCodes.CR)
                {
                    keyInfo = new ConsoleKeyInfo(onechar[0], ConsoleKey.Enter, false, false, false);
                }
                // Special key, e.g.: ! " # $ %
                else if ((b >= 32 && b <= 47) || (b >= 58 && b <= 64) || (b >= 91 && b <= 96) || (b >= 123 && b <= 126))
                {
                    keyInfo = new ConsoleKeyInfo(onechar[0], (ConsoleKey)0, false, false, false);
                }

                return keyInfo;
            }
            else
            {
                return Console.ReadKey();
            }
        }

        #endregion

        #endregion


        #region (private) Interaction <- main work happens here!

        private void Interaction()
        {

            ConsoleKeyInfo? Input;

            /*
            var cats = PerformanceCounterCategory.GetCategories();

            foreach (PerformanceCounterCategory cat in cats)
            {
                Console.WriteLine("Category: " + cat.CategoryName);

                string[] instances = cat.GetInstanceNames();
                if (instances.Length == 0)
                {
                    //foreach (PerformanceCounter ctr in cat.GetCounters())
                    //    Console.WriteLine("  Counter: " + ctr.CounterName);
                }
                else   // Dump counters with instances   
                {
                    foreach (string instance in instances)
                        if (instance.StartsWith("StudiVZ"))
                    {
                        Console.WriteLine("  Instance: " + instance);
                        if (cat.InstanceExists(instance))
                            foreach (PerformanceCounter ctr in cat.GetCounters(instance))
                                Console.WriteLine("    Counter: " + ctr.CounterName);
                        //Console.ReadLine();
                    }
                }

                //if (cat.CategoryName.Equals("Process"))
                //    Console.ReadLine();

            }
      
            _CPUCounter = new PerformanceCounter();
            //_CPUCounter.CategoryName    = "Processor";
            //_CPUCounter.CounterName     = "% Processor Time";
            //_CPUCounter.InstanceName    = "_Total";
            _CPUCounter.CategoryName = "Process";
            _CPUCounter.CounterName = "% Processor Time";
            _CPUCounter.InstanceName = Process.GetCurrentProcess().ProcessName;

            _RAMCounter = new PerformanceCounter(); //"Memory", "Available MBytes");
            _RAMCounter.CategoryName = "Process";
            _RAMCounter.CounterName = "Working Set - Private";
            _RAMCounter.InstanceName = Process.GetCurrentProcess().ProcessName;
      */


            //var kk = Process.GetCurrentProcess();
            //var mm = kk.TotalProcessorTime;
            //var ll = kk.WorkingSet64;

            while (!IsQuit)
            {

                #region Colors and Input

                //HACK: Strange console-encoding, otherwise Björn would be unreachable :(
                Console.InputEncoding = System.Text.Encoding.Default;

                //Prompt
                WriteLine();
                default_color = Console.ForegroundColor;
                WritePromptToConsole(default_color, CurrentPath);

                #endregion

                #region New Input

                while (!IsQuit) // replaced True with !IsQuit because telnet send a quit silently
                {

                    int NumberOfCharsPre = Console.CursorLeft;

                    #region Get Input

                    //get new Input from keyboard
                    Input = GetConsoleKeyInfo();
                    if (!Input.HasValue) // if the telnet connection timedout we have no valid key and should quit the CLI
                    {
                        IsQuit = true;
                        break;
                    }

                    #endregion

                    int NumberOfCharsPost = Console.CursorLeft;

                    #region Handle Input

                    if (Input.Value.IsSpecialKey())
                    {

                        #region Special Keys

                        switch (Input.Value.Key)
                        {
                            case ConsoleKey.UpArrow:

                            case ConsoleKey.DownArrow:
                                #region Handle Command History browsing

                                if (LoadStandardHistory)
                                {
                                    UseGraphHistory(ref NothingMountedList, Input);
                                }
                                else
                                {
                                    UseGraphHistory(ref SthMountedList, Input);
                                }

                                #endregion

                                currentCursorPosWithinCommand = InputString.Length - 1;
                                break;

                            case ConsoleKey.Enter:
                                #region Handle Return

                                //save command in history
                                if (!InputString.Length.Equals(0))
                                {
                                    //new history handling
                                    String TempInputString = InputString.Clone().ToString();

                                    if (InputString.Length.CompareTo(MaximumHistoryEntryLength) > 0)
                                    {
                                        TempInputString = TempInputString.Substring(0, MaximumHistoryEntryLength);
                                    }

                                    if (LoadStandardHistory)
                                    {
                                        NothingMountedList.Add(TempInputString);
                                    }
                                    else
                                    {
                                        SthMountedList.Add(TempInputString);
                                    }
                                }
                                EnterPressed = true;
                                PositionInHistory = 0;
                                currentCursorPosWithinCommand = 0;

                                #endregion
                                break;

                            case ConsoleKey.Backspace:
                                #region Handle Backspace
                                if (!InputString.Length.Equals(0))
                                {
                                    currentCursorPosWithinCommand--;
                                }

                                HandleBackSpace(NumberOfCharsPre, NumberOfCharsPost);
                                PositionInHistory = 0;

                                #endregion
                                break;

                            case ConsoleKey.Escape:
                                #region Handle Esc

                                ResetInputLineAndString();
                                PositionInHistory = 0;

                                #endregion

                                currentCursorPosWithinCommand = 0;
                                break;

                            case ConsoleKey.Tab:
                                #region Handle Tab

                                if (!AreWeInAnAutocompletionState())
                                {

                                    _CompletionList = GetPossibleTokensViaIrony(InputString);

                                    #region process possible tokens

                                    switch (_CompletionList.Count)
                                    {
                                        case 0:

                                            #region Nothing to do

                                            ResetInputLine();
                                            Write(InputString);

                                            #endregion

                                            break;

                                        case 1:

                                            #region Ac handling

                                            if (_CompletionList[0].isUsedForAutoCompletion)
                                            {
                                                //here we have an extended Autocompletion, because of the '\b' (by IRONY)

                                                if (!_CompletionList[0].typeOfLiteral.Equals(typeof(NumberLiteral)))
                                                {

                                                    #region Trigger extended autocompletion

                                                    InputString = InputString + "'";
                                                    ResetInputLine();
                                                    Write(InputString);
                                                    currentCursorPosWithinCommand++;

                                                    UseAutocompletion();

                                                }
                                                else
                                                {
                                                    ResetInputLine();
                                                    Write(InputString);
                                                    WriteLine(Environment.NewLine + Environment.NewLine + "Please type a number." + Environment.NewLine);
                                                    WriteLastInputstringToConsole(InputString);
                                                }

                                                    #endregion

                                            }
                                            else
                                            {
                                                ResetInputLine();

                                                InputString = InputString + _CompletionList[0].name;

                                                Write(InputString);

                                                #region update position in string

                                                currentCursorPosWithinCommand = currentCursorPosWithinCommand + _CompletionList[0].name.Length;

                                                #endregion
                                            }

                                            #endregion

                                            break;

                                        default:

                                            #region some more options

                                            PrintFilteredCommands(_CompletionList);
                                            WritePromptToConsole(default_color, CurrentPath);
                                            Write(InputString);

                                            #endregion

                                            break;

                                    }

                                    #endregion

                                }//are we in the extended AC-State?
                                else
                                {
                                    #region Trigger extended autocompletion

                                    UseAutocompletion();

                                    #endregion
                                }

                                #endregion

                                PositionInHistory = 0;

                                break;

                            case ConsoleKey.LeftArrow:
                                #region Handle LeftArrow

                                //Todo: Handle LeftArrow correct
                                if (IsTelnetCLI)
                                {
                                    //// if we are still after the prompt
                                    //if (ConsoleCursorLeft - ConsolePrompt.Length > 0)
                                    //{
                                    //    TelnetWrite(new Byte[] { 27, 91, 68 }, 0, 3);
                                    //    ConsoleCursorLeft--;
                                    //    Console.WriteLine("pos after left: " + ConsoleCursorLeft);
                                    //}
                                }

                                #endregion

                                if (!currentCursorPosWithinCommand.Equals(0))
                                {
                                    currentCursorPosWithinCommand--;
                                }

                                break;

                            case ConsoleKey.RightArrow:
                                #region Handle RightArrow

                                //Todo: Handle RightArrow correct

                                #endregion

                                if (!InputString.Length.Equals(currentCursorPosWithinCommand))
                                {
                                    currentCursorPosWithinCommand++;
                                }

                                break;

                            default:
                                ResetInputLine();
                                Write(InputString);
                                break;

                        }

                        #endregion
                    }
                    else
                    {
                        #region Char Keys

                        switch (Input.Value.KeyChar)
                        {

                            case '?':
                                #region Handle ?

                                if (InputString.Length.Equals(0))
                                {
                                    WriteLine(Environment.NewLine + "The following commands are available:" + Environment.NewLine);

                                    foreach (KeyValuePair<String, AllCLICommands> aCommand in Commands)
                                    {
                                        WriteLine("  {0,-20}{1}", aCommand.Value.Command, aCommand.Value.ShortInfo);
                                    }
                                    //Prompt
                                    WriteLine();
                                    default_color = Console.ForegroundColor;
                                    WritePromptToConsole(default_color, CurrentPath);
                                }
                                else
                                {
                                    //if the "?" is pressed during command typing, an detailed information concerning the 
                                    //current/following parameters should be shown
                                    WriteLine();
                                    WriteLastInputstringToConsole(InputString);
                                }

                                #endregion
                                break;

                            case '\'':

                                //This case is usefull for 2 cases. The first case closes an Ac by the CLI-user.
                                //The second one emerges when there are ambiguities during ac (for example 
                                //if there is the possibility to choose and stringLiteral or an simple keyword. 
                                //When someone likes to choose the stringLiteral, the correct Ac has to be executed.

                                #region Update InputString

                                InputString += Input.Value.KeyChar.ToString();
                                currentCursorPosWithinCommand++;

                                #endregion

                                break;

                            default:
                                #region Update InputString

                                InputString += Input.Value.KeyChar.ToString();

                                #endregion

                                if (IsTelnetCLI)
                                {
                                    //TelnetWriteByte((Byte)Input.Value.KeyChar);
                                    //ConsoleCursorLeft++;
                                }
                                currentCursorPosWithinCommand++;
                                PositionInHistory = 0;
                                break;

                        }

                        #endregion
                    }

                    #endregion

                    if (EnterPressed.Equals(true))
                    {
                        EnterPressed = false;
                        break;
                    }

                }//while

                #endregion

                ReadAndExecuteCommand(InputString);

            }


            #region Shutdown

            if (IsTelnetCLI)
            {
                //WriteLine("bye bye.");
                //TelnetClose();
            }
            else
            {
                WriteLine("bye bye.");
            }

            if (ShutDownOnExit)
            {

                if (_GraphDSSharp != null && Commands.ContainsKey("SHUTDOWNDB"))
                {
                    Commands["SHUTDOWNDB"].Execute(_GraphDSSharp, ref CurrentPath, null, null);
                }

                if (_GraphDSSharp != null && Commands.ContainsKey("UNMOUNTALL"))
                {
                    Commands["UNMOUNTALL"].Execute(_GraphDSSharp, ref CurrentPath, null, null);
                }

            }

            //save History
            if (!HistoryFileName.Length.Equals(0))
                SaveHistory(HistoryFileName, SthMountedList);

            #endregion

        }

        public void ReadAndExecuteCommand(String InputString)
        {

            // Read and execute commend

            #region Check if valid command
            //has to be done via split, because irony doesn't recognize whitespaces,
            //so "dfgfkgdfgkfd" could be detected as the command "df" with an 
            //strange parameter

            if (!IsQuit && ValidCommandFromInputString(InputString))
            {

            #endregion

                #region Prepare Command Execution

                _Scanner = GraphCLICompiler.Scanner;

                _CompilerContext = new CompilerContext(GraphCLICompiler);

                _SourceFile = new SourceFile(InputString, "Source");

                _Scanner.Prepare(_CompilerContext, _SourceFile);

                _CompilerContext.Tokens.Clear();

                _TokenStream = _Scanner.BeginNonDetermisticScan();

                AstNode ExecutionTree = null;

                ExecutionTree = GraphCLICompiler.Parser.ParseNonDeterministic(_CompilerContext, _TokenStream);

                #region Checkt if valid command is complete

                if (ExecutionTree == null)
                {
                    MarkWrongOption(InputString, GraphCLICompiler.Parser.GetCorrectElements(_CompilerContext, _TokenStream));
                }
                else
                {
                    //Carry on, the command is valid and complete
                #endregion

                    ExtractOptionsFromTree(ExecutionTree);

                #endregion

                    if (Commands[CurrentCommand].CLI_Output == CLI_Output.Standard)
                        WriteLine();

                    #region Handle Command Execution

                    //try
                    //{

                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        // TODO: what's this doing here? 
                        //if (Parameters.Count > 0)
                        //{
                        #region Execute command...

                        if (_GraphDSSharp != null || CurrentCommand.Equals("MKFS") || CurrentCommand.Equals("MOUNT") || CurrentCommand.Equals("QUIT") || CurrentCommand.Equals("EXIT") || CurrentCommand.Equals("USEHISTORY") || CurrentCommand.Equals("SAVEHISTORY"))
                        {


                            Commands[CurrentCommand].Execute(_GraphDSSharp, ref CurrentPath, Parameters, InputString);

                            //if (CommandCategory.Equals(CLICommandCategory.CLIStandardCommand))
                            //{

                                #region Handle Quit and History

                                switch (CurrentCommand.ToUpper())
                                {

                                    case "QUIT":
                                        IsQuit = true;
                                        break;

                                    case "EXIT":
                                        IsQuit = true;
                                        break;

                                    case "USEHISTORY":
                                        //lets move to the right parameter
                                        ParameterEnum = Parameters.GetEnumerator();
                                        ParameterEnum.MoveNext();
                                        ParameterEnum.MoveNext();

                                        switch (ParameterEnum.Current.Key)
                                        {
                                            case "default":
                                                LoadStandardHistory = true;

                                                if (!HistoryFileName.Length.Equals(0))
                                                    SaveHistory(HistoryFileName, SthMountedList);

                                                break;

                                            default:
                                                LoadStandardHistory = false;

                                                HistoryFileName = ParameterEnum.Current.Key;

                                                LoadHistoryFrom(HistoryFileName);

                                                break;

                                        }

                                        break;

                                    case "SAVEHISTORY":
                                        //lets move to the right parameter
                                        ParameterEnum = Parameters.GetEnumerator();
                                        ParameterEnum.MoveNext();
                                        ParameterEnum.MoveNext();

                                        if (LoadStandardHistory)
                                            SaveHistory(ParameterEnum.Current.Key, NothingMountedList);
                                        else
                                            SaveHistory(ParameterEnum.Current.Key, SthMountedList);

                                        break;

                                }

                                #endregion

                            //}

                        }

                        else
                            WriteLine("Nothing mounted...");

                        #endregion
                        //}//CommandArray.Length > 0 ?

                        sw.Stop();

                        if (Parameters.Count > 0 && Commands[CurrentCommand].CLI_Output != CLI_Output.Short)
                        {
                            WriteLine("Command took {0}ms, {1:0.0} MB RAM, {2:0.0}% CPU", sw.ElapsedMilliseconds, _RAMCounter.NextValue() / 1024 / 1024, _CPUCounter.NextValue());
                        }
                    //}
                    //catch (Exception e)
                    //{
                    //    WriteLine("Uuups... " + e.Message);
                    //    WriteLine("StackTrace... " + e.StackTrace);
                    //}

                    Reset();

                    #endregion

                }

            }

                

        }

        #endregion

        #region Run, RunAsThread

        #region Run()

        /// <summary>
        /// This method starts an interactive CLI session.
        /// </summary>
        public void Run()
        {

            Interaction();

        }

        #endregion

        #region RunAsThread()

        /// <summary>
        /// This method starts an interactive CLI session by creating
        /// a new thread.
        /// </summary>
        public void RunAsThread()
        {

            Thread _InteractiveThread = new Thread(new ThreadStart(Interaction));
            _InteractiveThread.Start();

        }

        #endregion

        #endregion


        #region Write(Line)

        public void Write(Boolean myValue)
        {
            Write(myValue ? "True" : "False");
        }

        public void Write(String myValue, params Object[] myArgs)
        {
            if (_StreamWriter != null)
            {
                _StreamWriter.Write(String.Format(myValue, myArgs));
                _StreamWriter.Flush();
            }
            else if (!IsTelnetCLI)
            {
                Console.Write(myValue, myArgs);
            }
            else
            {
                if (myArgs.Length > 0)
                    myValue = String.Format(myValue, myArgs);

                Byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes(myValue);
                //if (!StopRequested)
                    //TelnetWrite(bytesToSend, 0, bytesToSend.Length);
            }
        }

        public void WriteLine()
        {
            WriteLine(String.Empty);
        }

        public void WriteLine(Boolean myValue)
        {
            WriteLine((myValue ? "True" : "False"));
        }

        public void WriteLine(String myValue)
        {
            Write(myValue + Environment.NewLine);
        }

        public void WriteLine(String myValue, params Object[] myArgs)
        {
            Write(myValue + Environment.NewLine, myArgs);
        }

        #endregion

        #region SetCursorPosition(myLeft, myTop)

        private void SetCursorPosition(Int32 myLeft, Int32 myTop)
        {
            if (!IsTelnetCLI)
                Console.SetCursorPosition(myLeft, myTop);
        }

        #endregion

        #region Telnet

        //public override void ConnectionEstablished()
        //{

        //    #region Set event handler

        //    TelnetConnection.OnDontOptionReceived += new TelnetOptionEventHandler(TelnetParser_OnDontOptionReceived);
        //    TelnetConnection.OnDoOptionReceived += new TelnetOptionEventHandler(TelnetParser_OnDoReceived);
        //    TelnetConnection.OnWillOptionReceived += new TelnetOptionEventHandler(TelnetParser_OnWillOptionReceived);
        //    TelnetConnection.OnWontOptionReceived += new TelnetOptionEventHandler(TelnetParser_OnWontOptionReceived);
        //    TelnetConnection.OnDataToSendArrived += new TelnetDataEventHandler(TelnetParser_OnDataToSendArrived);
        //    TelnetConnection.OnSubnegotiationRecieved += new TelnetSubnegotiationEventHandler(TelnetParser_OnSubnegotiationRecieved);
        //    TelnetConnection.OnKeyReceived += new TelnetKeyEventHandler(TelnetParser_OnKeyReceived);

        //    #endregion

        //    #region Set CLI specific data

        //    Tuple<Object, String, Type[]> tupel = (Tuple<Object, String, Type[]>)DataObject;

        //    mountedVFS = tupel.TupelElement1;
        //    CurrentPath = tupel.TupelElement2;

        //    if (tupel.TupelElement3 != null)
        //        LoadGrammar(tupel.TupelElement3);
        //    else
        //        LoadGrammar();

        //    TelnetKeys = new Queue<ConsoleKeyInfo>();
        //    ShutDownOnExit = false;
        //    IsTelnetCLI = true;
            
        //    #endregion

        //    PrintStartupInformation();

        //    //#region Set and parse telnet options

        //    //TelnetConnection.UnsetOption(TelnetOptions.LineMode);
        //    //TelnetConnection.SetWillOption(TelnetOptions.Echo);

        //    //#endregion

        //    RunAsThread();

        //}

        //#region Events

        //void TelnetParser_OnSubnegotiationRecieved(object mySender, TelnetSubnegotiationEventArgs myEventArgs)
        //{
        //    if (myEventArgs.TelnetOption == TelnetOptions.WindowSize)
        //    {
        //        ConsoleWindowWidth = myEventArgs.ContentData[0] + myEventArgs.ContentData[1];
        //        ConsoleWindowHeight = myEventArgs.ContentData[2] + myEventArgs.ContentData[3];
        //    }
        //}

        //void TelnetParser_OnDataToSendArrived(object mySender, TelnetDataEventArgs myEventArgs)
        //{
        //    if (!StopRequested)
        //        TelnetWrite(myEventArgs.Data, 0, myEventArgs.Data.Length);
        //}

        //void TelnetParser_OnWontOptionReceived(object mySender, TelnetOptionEventArgs myEventArgs)
        //{
        //    myEventArgs.Accepted = true;
        //    //Console.WriteLine("[TelnetParser_OnWontOptionReceived] Command: " + myEventArgs.TelnetSymbol.Command + " Option: " + myEventArgs.TelnetSymbol.Option);
        //}

        //void TelnetParser_OnWillOptionReceived(object mySender, TelnetOptionEventArgs myEventArgs)
        //{
        //    //Console.WriteLine("[TelnetParser_OnWillOptionReceived] Command: " + myEventArgs.TelnetSymbol.Command + " Option: " + myEventArgs.TelnetSymbol.Option);
        //    if (myEventArgs.TelnetSymbol.Option == TelnetOptions.LineMode)
        //        myEventArgs.Accepted = false;
        //    if (myEventArgs.TelnetSymbol.Option == TelnetOptions.WindowSize)
        //        myEventArgs.Accepted = true;
        //}

        //void TelnetParser_OnDoReceived(object mySender, TelnetOptionEventArgs myEventArgs)
        //{
        //    myEventArgs.Accepted = true;
        //    //Console.WriteLine("[TelnetParser_OnDoReceived] Command: " + myEventArgs.TelnetSymbol.Command + " Option: " + myEventArgs.TelnetSymbol.Option);
        //}

        //void TelnetParser_OnDontOptionReceived(object mySender, TelnetOptionEventArgs myEventArgs)
        //{
        //    myEventArgs.Accepted = true;
        //    //Console.WriteLine("[TelnetParser_OnDontOptionReceived] Command: " + myEventArgs.TelnetSymbol.Command + " Option: " + myEventArgs.TelnetSymbol.Option);
        //}

        //void TelnetParser_OnKeyReceived(object mySender, TelnetKeyEventArgs myEventArgs)
        //{

        //    Monitor.Enter(TelnetKeys);
        //    TelnetKeys.Enqueue(myEventArgs.KeyInfo);
        //    Monitor.Pulse(TelnetKeys);
        //    Monitor.Exit(TelnetKeys);

        //}

        //#endregion

        //public override void ConnectionTimeout()
        //{
        //    // If we did not quit with "exit" command
        //    if (!IsQuit)
        //    {
        //        WriteLine("Connection timedout after " + Timeout + "ms...");
        //        IsQuit = true;

        //        // Pulse TelnetKeys to break the ReadKey loop in GetConsoleKeyInfo()
        //        Monitor.Enter(TelnetKeys);
        //        Monitor.Pulse(TelnetKeys);
        //        Monitor.Exit(TelnetKeys);
        //    }
        //}

        #endregion

    }

}
