
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
        Exceptional<List<String>> ExportGraphDDL(DumpFormats myDumpFormat, DBContext myDBContext, IEnumerable<GraphDBType> myTypes);
        Exceptional<List<String>> ExportGraphDML(DumpFormats myDumpFormat, DBContext myDBContext, IEnumerable<GraphDBType> myTypes);
    }
}
