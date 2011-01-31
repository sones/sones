using System;
using System.Collections.Generic;

using sones.GraphDB.Transaction;
using sones.Library.Internal.Security;

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
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <returns>A list of strings, containing the GDDL statements</returns>        
        IEnumerable<String> ExportGraphDDL(SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// Exports as GDML (data manipulation language)
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <returns>A list of strings, containing the GDML statments</returns>
        IEnumerable<String> ExportGraphDML(SecurityToken mySecurityToken, TransactionToken myTransactionToken);
    }
}
