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
 * GraphDSExtensionsCLI
 * Achim Friedland, 2010
 */

#region Usings

using System;

using sones.GraphDS.API.CSharp;
using sones.GraphDB.Connectors.GraphDBCLI;
using sones.GraphFS.Connectors.GraphFSCLI;
using sones.GraphFS.Connectors.GraphDSCLI;

using sones.Lib.CLI;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Extension methods to start a GraphDS CLI
    /// </summary>
    public static class GraphDSExtensionsCLI
    {

        #region OpenCLI()

        public static void OpenCLI(this GraphDSSharp myGraphDSSharp)
        {

            var _GraphDBCLI = new sonesCLI(
                                myGraphDSSharp.IGraphDBSession,
                                myGraphDSSharp.IGraphFSSession,
                                myGraphDSSharp.DatabaseName,
                                typeof(AllBasicFSCLICommands),
                                typeof(AllAdvancedFSCLICommands),
                                typeof(AllBasicDBCLICommands),
                                typeof(AllAdvancedDBCLICommands),
                                typeof(AllGraphDSCLICommands)
                              );

            _GraphDBCLI.Run();

        }

        #endregion

        #region OpenCLI(params myCommandTypes)

        public static void OpenCLI(this GraphDSSharp myGraphDSSharp, params Type[] myCommandTypes)
        {

            var _GraphDBCLI = new sonesCLI(
                                myGraphDSSharp.IGraphDBSession,
                                myGraphDSSharp.IGraphFSSession,
                                myGraphDSSharp.DatabaseName,
                                myCommandTypes
                              );

            _GraphDBCLI.Run();

        }

        #endregion

    }

}
