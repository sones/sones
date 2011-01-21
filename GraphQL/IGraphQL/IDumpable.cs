using System;
using System.Collections.Generic;

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
        /// <returns>A list of strings, containing the GDDL statements</returns>        
        IEnumerable<String> ExportGraphDDL();

        /// <summary>
        /// Exports as GDML (data manipulation language)
        /// </summary>
        /// <returns>A list of strings, containing the GDML statments</returns>
        IEnumerable<String> ExportGraphDML();
    }
}
