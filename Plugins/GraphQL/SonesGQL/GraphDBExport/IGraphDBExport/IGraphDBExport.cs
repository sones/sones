using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;

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
    public interface IGraphDBExport
    {
        string ExportFormat { get; }

        string ExporterName { get; }

        QueryResult Export(String destination, IDumpable myGrammar, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IEnumerable<String> myTypes, DumpTypes myDumpType);
    }
}
