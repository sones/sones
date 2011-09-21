/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.DBExport
{
    #region IGraphDBExportVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBExport plugin versions. 
    /// Defines the min and max version for all IGraphDBExport implementations which will be activated used this IGraphDBExport.
    /// </summary>
    public static class IGraphDBExportVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// The interface for a GraphDBExporter
    /// </summary>
    public interface IGraphDBExport : IPluginable
    {
        string ExporterName { get; }

        QueryResult Export(String destination, 
                            IDumpable myGrammar, 
                            IGraphDB myGraphDB, 
                            IGraphQL myGraphQL, 
                            SecurityToken mySecurityToken, 
                            Int64 myTransactionToken, 
                            IEnumerable<String> myVertexTypes,
                            IEnumerable<String> myEdgeTypes, 
                            DumpTypes myDumpType);
    }
}
