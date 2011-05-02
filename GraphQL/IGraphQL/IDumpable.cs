using System;
using System.Collections.Generic;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Library.DataStructures;

namespace sones.GraphQL
{
    /// <summary>
    /// Marks a grammar as dump able
    /// </summary>
    public interface IDumpable
    {
        /// <summary>
        /// Export as GDDL (data definition language)
        /// </summary>
        /// <param name="myTypesToDump">The types to dump</param>
        /// <returns>A list of strings, containing the GDDL statements</returns>        
        IEnumerable<String> ExportGraphDDL(DumpFormats myDumpFormat, IEnumerable<IVertexType> myTypesToDump);

        /// <summary>
        /// Exports as GDML (data manipulation language)
        /// </summary>
        /// <param name="myTypesToDump">The types to dump</param>
        /// <returns>A list of strings, containing the GDML statments</returns>
        IEnumerable<String> ExportGraphDML(DumpFormats myDumpFormat, IEnumerable<IVertexType> myTypesToDump, SecurityToken mySecurityToken, TransactionToken myTransactionToken);
    }
}