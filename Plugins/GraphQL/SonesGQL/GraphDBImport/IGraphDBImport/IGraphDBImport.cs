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
