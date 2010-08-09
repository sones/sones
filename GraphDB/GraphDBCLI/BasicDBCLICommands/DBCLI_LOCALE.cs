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


#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.ErrorHandling;
using System.IO;
using System.Diagnostics;

using sones.GraphDB;
using sones.GraphFS.Session;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Sets a locale
    /// </summary>

    public class DBCLI_LOCALE : AllBasicDBCLICommands
    {
        
        #region Constructor

        public DBCLI_LOCALE()
        {

            // Command name and description
            InitCommand("LOCALE",
                        "Sets the CLI locale",
                        "Sets the CLI locale");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralExternalEntry);

            this.OnCancelRequested += new EventHandler(DBCLI_LOCALE_OnCancelRequested);

        }

        void DBCLI_LOCALE_OnCancelRequested(object sender, EventArgs e)
        {
            _CancelCommand = true;
        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFS2Session, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {
            
            var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IPandoraDBSession = myIPandoraDBSession as IGraphDBSession;

            Stopwatch sw = new Stopwatch();
            String locale = myOptions.ElementAt(1).Value[0].Option;

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(locale);
            }
            catch
            {
            }
            Console.WriteLine("Current Thread-Culture is: "+System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        private bool ExecuteAQuery(string query, IGraphDBSession dbSession)
        {
            if (query.Length != 0)
            {
                //execute query
                QueryResult myQueryResult = QueryDB(query, dbSession);

                WriteLine(query);

                HandleQueryResult(myQueryResult, false);

                if (myQueryResult.ResultType != ResultType.Successful)
                {
                    WriteLine(myQueryResult.GetErrorsAsString());
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        #endregion

    }

}
