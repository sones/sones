/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* Pandora CLI - AllCLICommands
 * (c) Henning Rauch, 2009
 * 
 * The abstract class for all commands of the grammar-based
 * command line interface.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using sones.Lib;
using sones.Lib.Frameworks.CLIrony.Compiler;

#endregion

namespace sones.Lib.CLI
{

    public enum CLI_Output
    {
        Standard,
        Short
    }

    public abstract class AllCLICommands : Grammar
    {

        #region Events

        public event EventHandler OnCancelRequested;
        /// <summary>
        /// This method will invoke any OnCancelRequested event. If there is no event, it will return True to cancel the CLI process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public Boolean CancelCurrentCommand(Object sender, EventArgs eventArgs)
        {

            if (OnCancelRequested != null)
            {
                _CancelCommand = true;
                OnCancelRequested(sender, eventArgs);
                return true;
            }

            return false;

        }

        #endregion

        /// <summary>
        /// The abstract class for all commands of the grammar-based
        /// command line interface.
        /// </summary>

        #region Data

        #region Global grammar data

        #region Global grammar lists

        private List<SymbolTerminal> _GlobalSymbolTerminalList  = new List<SymbolTerminal>();
        private List<NumberLiteral>  _GlobalNumberLiteralList   = new List<NumberLiteral>();
        private List<StringLiteral>  _GlobalStringLiteralList   = new List<StringLiteral>();

        #endregion

        #region Terminals



        #endregion

        #region Symbols

        protected SymbolTerminal Eq_Less_Or_Equal       = Symbol("<=");
        protected SymbolTerminal Eq_Greater_Or_Equal    = Symbol(">=");
        protected SymbolTerminal Eq_Less                = Symbol("<");
        protected SymbolTerminal Eq_Greater             = Symbol(">");
        protected SymbolTerminal Eq_Equals              = Symbol("=");

        protected SymbolTerminal MultiplicatorSizeByte  = Symbol("B");
        protected SymbolTerminal MultiplicatorSizeMega  = Symbol("M");
        protected SymbolTerminal MultiplicatorSizeGiga  = Symbol("G");
        protected SymbolTerminal MultiplicatorSizeTera  = Symbol("T");

        protected SymbolTerminal DateToday              = Symbol("TODAY");
        protected SymbolTerminal DateYesterday          = Symbol("YESTERDAY");

        protected SymbolTerminal SortByName             = Symbol("NAME");
        protected SymbolTerminal SortByDate             = Symbol("DATE");
        protected SymbolTerminal SortBySize             = Symbol("SIZE");

        protected SymbolTerminal OrSymbol               = Symbol("|");
        protected SymbolTerminal SemicolonSymbol             = Symbol(";");
        protected SymbolTerminal CommaSymbol             = Symbol(",");

        protected SymbolTerminal DotSymbol              = Symbol(".");
        protected SymbolTerminal ColonSymbol            = Symbol(":");

        protected SymbolTerminal Protocol_File          = Symbol("file://");
        protected SymbolTerminal Protocol_Ftp           = Symbol("ftp://");
        protected SymbolTerminal Protocol_Http          = Symbol("http://");
        protected SymbolTerminal Protocol_Sftp          = Symbol("sftp://");

        protected SymbolTerminal DefaultSymbol          = Symbol("default");
        protected SymbolTerminal CRSymbol               = Symbol("<CR>");
        protected SymbolTerminal LoadSymbol             = Symbol("load");
        protected SymbolTerminal StoreSymbol            = Symbol("store");
        protected SymbolTerminal StartSymbol            = Symbol("start");
        protected SymbolTerminal StopSymbol             = Symbol("stop");
        protected SymbolTerminal BracketRoundOpenSymbol       = Symbol("(");
        protected SymbolTerminal BracketRoundCloseSymbol       = Symbol(")");

        #endregion

        #region Literals

        private NumberLiteral _numberLiteral = new NumberLiteral("numberLiteral", NumberFlags.IntOnly);
        protected StringLiteral stringLiteral = new StringLiteral("stringLiteral", "'", StringFlags.AllowsDoubledQuote);
        private StringLiteral _stringLiteral_IPandoraFS = new StringLiteral("stringLiteral_IPandoraFS", "'", StringFlags.AllowsDoubledQuote);
        private StringLiteral _stringLiteral_ExternalEntry = new StringLiteral("stringLiteral_ExternalEntry", "'", StringFlags.AllowsDoubledQuote);
        
        #endregion

        #region NonTerminals

        private NonTerminal _Equalisator        = new NonTerminal("Equalisator");
        private NonTerminal _MultiplicatorSize  = new NonTerminal("MultiplicatorSize");
        private NonTerminal _DateTimeGrammar    = new NonTerminal("DateTimeGrammar");
        private NonTerminal _TimeGrammar        = new NonTerminal("TimeGrammar");
        private NonTerminal _DateGrammar        = new NonTerminal("DateGrammar");
        private NonTerminal _LinkedStringList   = new NonTerminal("LinkedStringList");
        private NonTerminal _Protocol           = new NonTerminal("Protocol");

        #endregion

        #endregion

        #region Typed lists

        protected List<Terminal>       _CommandTerminals       = new List<Terminal>();
        protected List<NonTerminal>    _CommandNonTerminals    = new List<NonTerminal>();
        protected List<SymbolTerminal> _CommandSymbolTerminal  = new List<SymbolTerminal>();

        #endregion

        #region Command Grammar Subtree

        protected Grammar _CommandGrammar = new Grammar();

        #endregion



        public Grammar CommandGrammar
        {

            get
            {

                _CommandGrammar.Terminals.AddRange(_CommandTerminals);


                foreach (NonTerminal aNonTerminal in _CommandNonTerminals)
                {
                    if (!aNonTerminal.PandoraOptions.Contains(PandoraOption.IsOption))
                    {
                        aNonTerminal.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                    }
                }

                _CommandGrammar.NonTerminals.AddRange(_CommandNonTerminals);

                foreach (SymbolTerminal _aCommandSymbolTerminal in _CommandSymbolTerminal)
                {
                    _CommandGrammar.Terminals.Add(_aCommandSymbolTerminal);
                }

                _CommandGrammar.Root = CLICommandNonTerminal;

                return _CommandGrammar;

            }

        }



        #region Fields

        sonesCLI graphCLI;
        String PathDelimiter = "/";
        String DotDotLink = "..";
        #endregion

        #endregion

        #region Properties

        #region General Command Infos

        protected Boolean _CancelCommand = false;

        protected String _Command = String.Empty;
        protected String _ShortInfo = String.Empty;
        protected String _Information = String.Empty;
        
        public String Command      { get { return _Command; } }
        public String ShortInfo    { get { return _ShortInfo; } }
        public String Information  { get { return _Information; } }

        public CLI_Output CLI_Output { get; set; } 

        // Non-terminal declaration
        protected NonTerminal CLICommandNonTerminal;

        // Symbol declaration
        protected SymbolTerminal CLICommandSymbolTerminal;

        // Terminal declaration


        #endregion

        #region Command Grammar

        //public abstract Grammar CommandGrammar { get; }

        #endregion

        #region Global NonTerminals

        protected NonTerminal Protocol
        {

            get
            {

                _Protocol.Rule =        Protocol_File
                                    |   Protocol_Ftp
                                    |   Protocol_Http
                                    |   Protocol_Sftp;
                _Protocol.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                return _Protocol;

            }

        }

        protected NonTerminal Equalisator
        {

            get
            {

                _Equalisator.Rule =         Eq_Less_Or_Equal
                                        |   Eq_Greater_Or_Equal
                                        |   Eq_Less
                                        |   Eq_Greater
                                        |   Eq_Equals;
                _Equalisator.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                return _Equalisator;

            }

        }

        protected NonTerminal MultiplicatorSize
        {

            get
            {

                _MultiplicatorSize.Rule = MultiplicatorSizeByte
                                            | MultiplicatorSizeMega
                                            | MultiplicatorSizeGiga
                                            | MultiplicatorSizeTera;
                _MultiplicatorSize.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                return _MultiplicatorSize;

            }

        }

        protected NonTerminal DateTimeGrammar
        {

            get
            {

                _DateTimeGrammar.Rule =     TimeGrammar
                                        |   DateGrammar
                                        |   TimeGrammar + DateGrammar;
                _DateTimeGrammar.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                return _DateTimeGrammar;

            }

        }

        protected NonTerminal TimeGrammar
        {

            get
            {

                _TimeGrammar.Rule = numberLiteral + ColonSymbol + numberLiteral;
                _TimeGrammar.IsUsedForAutocompletion = true;
                _TimeGrammar.Description = "Time (Hour:Minute)";

                return _TimeGrammar;

            }

        }

        protected NonTerminal DateGrammar
        {

            get
            {

                _DateGrammar.Rule = numberLiteral + DotSymbol + numberLiteral + DotSymbol + numberLiteral
                                    |   DateToday
                                    |   DateYesterday;
                _DateGrammar.PandoraOptions.Add(PandoraOption.IsStructuralObject);
                _DateGrammar.IsUsedForAutocompletion = true;
                _DateGrammar.Description = "Date (Day.Month.Year) | " + DateToday.Name + " | " + DateYesterday.Name;

                return _DateGrammar;

            }

        }

        protected NonTerminal LinkedStringList
        {

            get
            {

                _LinkedStringList.Rule =        stringLiteral + OrSymbol + _LinkedStringList
                                            |   stringLiteral + SemicolonSymbol;
                _LinkedStringList.PandoraOptions.Add(PandoraOption.IsStructuralObject);

                return _LinkedStringList;

            }

        }

        protected StringLiteral stringLiteralPVFS
        {

            get
            {
                _stringLiteral_IPandoraFS.Description = "DIRECTORY \t (Autocompletion for PVFS directories.)";
                _stringLiteral_IPandoraFS.PandoraOptions.Add(PandoraOption.IsUsedForAutocompletion);

                return _stringLiteral_IPandoraFS;
            }

        }
        
        protected StringLiteral stringLiteralExternalEntry
        {

            get
            {
                _stringLiteral_ExternalEntry.Description = "DIRECTORY \t (Autocompletion for external directories.)";
                _stringLiteral_ExternalEntry.PandoraOptions.Add(PandoraOption.IsUsedForAutocompletion);

                return _stringLiteral_ExternalEntry;

            }

        }

        protected NumberLiteral numberLiteral
        {

            get
            {
                _numberLiteral.Description = "Number \t (Type sth like a 10 or so.)";

                return _numberLiteral;

            }

        }
        
        #endregion

        //#region Global command categories

        //public abstract CLICommandCategory aCategory { get; }

        //#endregion

        #endregion


        protected void InitCommand(String myCommand, String myShortInfo, String myInformation)
        {

            _Command      = myCommand;
            _ShortInfo    = myShortInfo;
            _Information  = myInformation;

            CLICommandNonTerminal = new NonTerminal(_Command);
            CLICommandSymbolTerminal = Symbol(_Command);

        }


        protected void CreateBNFRule(BnfExpression myBnfExpression)
        {

            CLICommandNonTerminal.Rule = myBnfExpression;
            CLICommandNonTerminal.PandoraOptions.Add(PandoraOption.IsCommandRoot);

            #region Todo: via reflection

            // Non-terminal integration
            _CommandNonTerminals.Add(CLICommandNonTerminal);

            // Symbol integration
            _CommandSymbolTerminal.Add(CLICommandSymbolTerminal);
            
            #endregion

        }


        #region (public) Methods

        #region execute Method

        public abstract void Execute(ref object myPVFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString);

        #endregion

        #region GetObjectLocation(myCurrentPath, myObjectName)

        /// <summary>
        /// Takes the current path and an input and returns the resulting
        /// object location without any "." and ".." removal.
        /// </summary>
        /// <param name="myCurrentPath">the current path</param>
        /// <param name="myObjectName">an object to access</param>
        /// <returns>the resulting object location</returns>
        protected String GetObjectLocation(String myCurrentPath, String myObjectName)
        {

            if (myObjectName.StartsWith(PathDelimiter))
                return myObjectName;

            else

                if (myCurrentPath.Equals(PathDelimiter))
                    return String.Concat(PathDelimiter, myObjectName);

                else
                    return String.Concat(myCurrentPath, PathDelimiter, myObjectName);

        }

        #endregion

        #region SimplifyObjectLocation(myCurrentPath)

        /// <summary>
        /// Simplifies an object location by removing relative path fragments
        /// like /directoryname1/../directoryname2 -> /directoryname2
        /// </summary>
        /// <param name="myCurrentPath">the current path</param>
        /// <returns>a simplified current path</returns>
        public String SimplifyObjectLocation(String myCurrentPath)
        {

            String _newPath = "";
            String[] _PathSeperator = { PathDelimiter, "" };
            String[] _SplittedCurrentPath = myCurrentPath.Split(_PathSeperator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < _SplittedCurrentPath.Length; i++)
            {

                if (_SplittedCurrentPath[i].Equals(DotDotLink))
                {
                    _SplittedCurrentPath[i] = "";
                    _SplittedCurrentPath[i - 1] = "";
                }

                else if (_SplittedCurrentPath[i].Equals(PathDelimiter))
                {
                    _SplittedCurrentPath[i] = "";
                }

            }

            foreach (String _Path in _SplittedCurrentPath)
                if (!_Path.Equals("")) _newPath = String.Concat(_newPath, PathDelimiter, _Path);

            if (_newPath.Equals(""))
                _newPath = PathDelimiter;

            return _newPath;

        }

        #endregion

        #region SetCLIReference(PandoraCLI myPandoraCLI)

        public void SetCLIReference(sonesCLI myPandoraCLI)
        {
            graphCLI = myPandoraCLI;
        }

        #endregion

        #region Write(Line)

        public void Write(Boolean myValue)
        {
            graphCLI.Write(myValue);
        }

        public void Write(String myValue, params Object[] myArgs)
        {
            graphCLI.Write(myValue, myArgs);
        }

        public void WriteLine()
        {
            graphCLI.WriteLine();
        }

        public void WriteLine(Boolean myValue)
        {
            graphCLI.WriteLine(myValue);
        }

        public void WriteLine(String myValue)
        {
            graphCLI.WriteLine(myValue);
        }

        public void WriteLine(String myValue, params Object[] myArgs)
        {
            graphCLI.WriteLine(myValue, myArgs);
        }

        #endregion

        #endregion

    }

    #region Global command categories

    public enum CLICommandCategory
    {
        None = 0,
        AdvancedFSCLICommand = 0x01,
        BasicFSCLICommand = 0x02,
        ScriptingFSCLICommand = 0x04,
        FSCommand = 0x07,
        BasicDBCLICommand = 0x08,
        AdvancedDBCLICommand = 0x10,
        DBCommand = 0x38,
        CLIStandardCommand = 0x40,
    }

    #endregion

}
