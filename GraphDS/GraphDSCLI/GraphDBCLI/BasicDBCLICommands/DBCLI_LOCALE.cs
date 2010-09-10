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
 * DBCLI_LOCALE
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphDB.Structures;

using sones.GraphDS.Connectors.CLI;
using sones.GraphDBInterface.Result;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            var sw = new Stopwatch();
            var locale = myOptions.ElementAt(1).Value[0].Option;

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(locale);
            }
            catch
            {
            }
            
            Console.WriteLine("Current Thread-Culture is: "+System.Threading.Thread.CurrentThread.CurrentCulture);

            return Exceptional.OK;

        }

        private Boolean ExecuteAQuery(AGraphDSSharp myAGraphDSSharp, String myQueryString)
        {

            if (myQueryString.Length != 0)
            {

                var myQueryResult = QueryDB(myAGraphDSSharp, myQueryString);

                WriteLine(myQueryString);

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
