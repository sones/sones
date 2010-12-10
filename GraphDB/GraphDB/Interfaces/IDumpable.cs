
#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ImportExport;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Interfaces
{

    /// <summary>
    /// Marks a grammar as dump able
    /// </summary>
    public interface IDumpable
    {
        /// <summary>
        /// Export as GDDL (data definition language)
        /// The dump result contains "CREATE VERTEX " statements
        /// </summary>
        /// <param name="myDumpFormat">The dumpformat</param>
        /// <param name="myDBContext">The dbcontext</param>
        /// <param name="myTypes">A list of types to dump</param>
        /// <returns>An exceptional with a list of strings, containing the GDDL statements</returns>        
        Exceptional<List<String>> ExportGraphDDL(DumpFormats myDumpFormat, DBContext myDBContext, IEnumerable<GraphDBType> myTypes);

        /// <summary>
        /// Exports as GDML (data manipulation language)
        /// The dump result contains only "INSERT INTO " statements
        /// </summary>
        /// <param name="myDumpFormat">The dumpformat</param>
        /// <param name="myDBContext">The dbcontext</param>
        /// <param name="myTypes">A list of types to dump</param>
        /// <returns>An exceptional with a list of strings, containing the GDML statments</returns>
        Exceptional<List<String>> ExportGraphDML(DumpFormats myDumpFormat, DBContext myDBContext, IEnumerable<GraphDBType> myTypes);
    }
}
