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

/* PandoraFS CLI - LS
 * (c) Henning Rauch, 2009
 * 
 * The "LS"-command of the grammar-based Command Line
 * Interface of the Pandora File System.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.Lib.Frameworks.CLIrony.Compiler;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Lists the content of a directory
    /// </summary>

    public class FSCLI_LS : AllBasicFSCLICommands
    {




        #region Constructor

        public FSCLI_LS()
        {

            // Command name and description
            InitCommand("LS",
                        "Lists the content of a directory",
                        "Lists the content of a directory");


            #region Symbol declaration

            SymbolTerminal LS_Option_Long_long = Symbol("--long");
            SymbolTerminal LS_Option_Long_name = Symbol("--Name");
            SymbolTerminal LS_Option_Long_regexpr = Symbol("--regexpr");
            SymbolTerminal LS_Option_Long_streamtype = Symbol("--streamtype");
            SymbolTerminal LS_Option_Long_ignoreStreamtype = Symbol("--ignore-streamtype");
            SymbolTerminal LS_Option_Long_size = Symbol("--size");
            SymbolTerminal LS_Option_Long_creationtime = Symbol("--creationtime");
            SymbolTerminal LS_Option_Long_modificationtime = Symbol("--modificationtime");
            SymbolTerminal LS_Option_Long_lastaccesstime = Symbol("--lastaccesstime");
            SymbolTerminal LS_Option_Long_deletiontime = Symbol("--deletiontime");
            SymbolTerminal LS_Option_Long_columsize = Symbol("--columsize");
            SymbolTerminal LS_Option_Long_color = Symbol("--color");
            SymbolTerminal LS_Option_Long_sort = Symbol("--sort");
            SymbolTerminal LS_Option_Long_reverse = Symbol("--reverse");
            SymbolTerminal LS_Option_Long_group = Symbol("--group");
            SymbolTerminal LS_Option_Long_all = Symbol("--all");
            SymbolTerminal LS_Option_Long_almostAll = Symbol("--almost-all");

            #endregion

            #region Non-terminal declaration

            NonTerminal LS_Options = new NonTerminal("LS_Options");
            NonTerminal LS_Option_List = new NonTerminal("LS_Option_List");
            NonTerminal LS_Option = new NonTerminal("LS_Option");
            NonTerminal LS_Option_Long = new NonTerminal("LS_Option_Long");
            //NonTerminal LS_Option_Short                   = new NonTerminal("LS_Option_Short");
            NonTerminal SortBy = new NonTerminal("SortBy");

            #endregion

            #region Non-terminal integration

            _CommandNonTerminals.Add(LS_Options);
            _CommandNonTerminals.Add(LS_Option_List);
            _CommandNonTerminals.Add(LS_Option);
            _CommandNonTerminals.Add(LS_Option_Long);
            _CommandNonTerminals.Add(SortBy);

            #endregion

            #region Symbol integration

            _CommandSymbolTerminal.Add(LS_Option_Long_long);
            _CommandSymbolTerminal.Add(LS_Option_Long_name);
            _CommandSymbolTerminal.Add(LS_Option_Long_regexpr);
            _CommandSymbolTerminal.Add(LS_Option_Long_streamtype);
            _CommandSymbolTerminal.Add(LS_Option_Long_ignoreStreamtype);
            _CommandSymbolTerminal.Add(LS_Option_Long_size);
            _CommandSymbolTerminal.Add(LS_Option_Long_creationtime);
            _CommandSymbolTerminal.Add(LS_Option_Long_modificationtime);
            _CommandSymbolTerminal.Add(LS_Option_Long_lastaccesstime);
            _CommandSymbolTerminal.Add(LS_Option_Long_deletiontime);
            _CommandSymbolTerminal.Add(LS_Option_Long_columsize);
            _CommandSymbolTerminal.Add(LS_Option_Long_color);
            _CommandSymbolTerminal.Add(LS_Option_Long_sort);
            _CommandSymbolTerminal.Add(LS_Option_Long_reverse);
            _CommandSymbolTerminal.Add(LS_Option_Long_group);
            _CommandSymbolTerminal.Add(LS_Option_Long_all);
            _CommandSymbolTerminal.Add(LS_Option_Long_almostAll);

            #endregion


            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal | CLICommandSymbolTerminal + LS_Options);

            #region Command Rules

//            LS.PandoraOptions.Add(PandoraOption.IsCommandRoot);

            LS_Options.Rule = stringLiteralPVFS + LS_Option_List
                                | LS_Option_List;
            LS_Options.PandoraOptions.Add(PandoraOption.IsOption);

            LS_Option_List.Rule = LS_Option + LS_Option_List
                                    | LS_Option;

            /*LS_Option.Rule =      LS_Option_Long 
                                |   LS_Option_Short;*/

            LS_Option.Rule = LS_Option_Long;

            #endregion

            #region LS long options

            LS_Option_Long.Rule = LS_Option_Long_long
                                    | LS_Option_Long_name + Eq_Equals + LinkedStringList
                                    | LS_Option_Long_regexpr + Eq_Equals + stringLiteral
                                    | LS_Option_Long_streamtype + Eq_Equals + LinkedStringList
                                    | LS_Option_Long_ignoreStreamtype + Eq_Equals + LinkedStringList
                                    | LS_Option_Long_size + Equalisator + numberLiteral + MultiplicatorSize
                                    | LS_Option_Long_creationtime + Equalisator + DateTimeGrammar
                                    | LS_Option_Long_modificationtime + Equalisator + DateTimeGrammar
                                    | LS_Option_Long_lastaccesstime + Equalisator + DateTimeGrammar
                                    | LS_Option_Long_deletiontime + Equalisator + DateTimeGrammar
                                    | LS_Option_Long_columsize + Eq_Equals + numberLiteral
                                    | LS_Option_Long_color
                                    | LS_Option_Long_sort + Eq_Equals + SortBy
                                    | LS_Option_Long_reverse
                                    | LS_Option_Long_group
                                    | LS_Option_Long_all
                                    | LS_Option_Long_almostAll;

            LS_Option_Long.PandoraOptions.Add(PandoraOption.IsOption);

            SortBy.Rule = SortByName
                            | SortByDate
                            | SortBySize;

            #endregion



        }

        #endregion


        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No file system mounted...");
                return;
            }

            var _GetDirectoryListingExceptional = ((IGraphFSSession)_IGraphFSSession).GetDirectoryListing(new ObjectLocation(myCurrentPath));

            if (_GetDirectoryListingExceptional != null && _GetDirectoryListingExceptional.Success && _GetDirectoryListingExceptional.Value != null)
            {
                foreach (var _DirectoryEntry in _GetDirectoryListingExceptional.Value)
                {
                    WriteLine(_DirectoryEntry);
                }
            }

            else
            {
                foreach (var _IError in _GetDirectoryListingExceptional.Errors)
                    WriteLine(_IError.Message);
            }


            WriteLine();


        }

        #endregion

    }

}
