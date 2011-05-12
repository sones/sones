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

namespace sones.Plugins.SonesGQL.DBImport
{
    #region IGraphDBImportVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBImport plugin versions. 
    /// Defines the min and max version for all IGraphDBImport implementations which will be activated used this IGraphDBImport.
    /// </summary>
    public static class IGraphDBImportVersionCompatibility
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
    /// The interface for a GraphDBImporter
    /// </summary>
    public interface IGraphDBImport
    {
        string ImportFormat { get; }

        string ImporterName { get; }

        QueryResult Import(String myLocation, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, UInt32 myParallelTasks = 1U, IEnumerable<string> myComments = null, UInt64? myOffset = null, UInt64? myLimit = null, VerbosityTypes myVerbosityTypes = VerbosityTypes.Silent);
    }
}
