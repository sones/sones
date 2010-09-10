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
 * AllGraphDSCLICommands
 * Achim Friedland, 2010
 */

#region Usings

using System;

using sones.GraphDB;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;

using sones.Lib.CLI;

#endregion

namespace sones.GraphFS.Connectors.GraphDSCLI
{

    /// <summary>
    /// The abstract class for all GraphDS commands of the grammar-based
    /// command line interface.
    /// </summary>
    public abstract class AllGraphDSCLICommands : AllCLICommands
    {

        #region QueryDB(myQueryString, IGraphDBSession, myWithOutput = true)

        protected QueryResult QueryDB(String myQueryString, IGraphDBSession myIGraphDBSession, Boolean myWithOutput = true)
        {

            if (myWithOutput)
                Write(myQueryString + " => ");

            var _QueryResult = myIGraphDBSession.Query(myQueryString);

            if (myWithOutput)
                WriteLine(_QueryResult.ResultType.ToString());

            if (_QueryResult == null)
                WriteLine("The QueryResult is invalid!\n\n");

            else if (_QueryResult.ResultType != ResultType.Successful)
                foreach (var aError in _QueryResult.Errors)
                    WriteLine(aError.GetType().ToString() + ": " + aError.ToString() + "\n\n");

            return _QueryResult;

        }

        #endregion
    
    }

}
