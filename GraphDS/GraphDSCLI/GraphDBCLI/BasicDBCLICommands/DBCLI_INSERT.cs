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
 * DBCLI_INSERT
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using sones.GraphFS.Session;

using sones.GraphDS.Connectors.CLI;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDBInterface.Result;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Inserts new Objects into the GraphDB
    /// </summary>

    public class DBCLI_INSERT : AllBasicDBCLICommands
    {

        #region Constructor

        public DBCLI_INSERT()
        {

            // Command name and description
            InitCommand("INSERT",
                        "Inserts new Objects into an instance of the GraphDB",
                        "Inserts new Objects into an instance of the GraphDB");

            #region BNF rule

            var INTO            = Symbol("INTO");
            var VALUES          = Symbol("VALUES");
            var AttrAssignList  = new NonTerminal("AttrAssignList");
            var AttrAssign      = new NonTerminal("AttrAssign");
            var StringOrNumber  = new NonTerminal("StringOrNumber");
            var comma           = Symbol(",");
            var gleich          = Symbol("=");
            var LISTOF          = Symbol("LISTOF");
            var SETOF           = Symbol("SETOF");
            var SETREF          = Symbol("SETREF");

            
            CreateBNFRule(CLICommandSymbolTerminal + INTO + stringLiteral + VALUES + "(" + AttrAssignList + ")");

            AttrAssignList.Rule = AttrAssign + comma + AttrAssignList
                                   | AttrAssign;

            AttrAssignList.GraphOptions.Add(GraphOption.IsStructuralObject);

            StringOrNumber.Rule = stringLiteral | numberLiteral;

            AttrAssign.Rule = stringLiteral + gleich + StringOrNumber
                                | stringLiteral + gleich + LISTOF + stringLiteral
                                | stringLiteral + gleich + SETOF + stringLiteral
                                | stringLiteral + gleich + SETREF + stringLiteral;

            AttrAssignList.GraphOptions.Add(GraphOption.IsOption);

            #endregion

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            QueryResult _QueryResult;

            myInputString = Regex.Replace(myInputString, "[INSERT|insert]+ [INTO|into]+ (\')(.*?)(\') [VALUES|values]+", "INSERT INTO $2 VALUES");
            myInputString = Regex.Replace(myInputString, "([\\(|,]+)(\\s*)(\')(.*?)(\')(\\s*)=", "$1 $4 =");

            _QueryResult = QueryDB(myAGraphDSSharp, myInputString);

            return Exceptional.OK;

        }

        #endregion

    }

}
