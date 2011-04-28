using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL;

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
    /// The interface for all GQL functions
    /// </summary>
    public interface IGraphDBImport
    {
        string ImportFormat { get; }

        QueryResult Import(String location, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, bool myBreakOnError = false, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null);

        QueryResult Import(Stream myInputStream, IGraphDB myIGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, bool myBreakOnError = false, UInt32 myParallelTasks = 1, IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null);
    }
}
