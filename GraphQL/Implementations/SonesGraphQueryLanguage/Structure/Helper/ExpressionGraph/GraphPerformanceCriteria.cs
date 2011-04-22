using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This enum lists some criteria of a graph
    /// </summary>
    public enum GraphPerformanceCriteria
    {
        Space,
        Time,
        LevelResolution,
        FastInsertion,
        FastDataExtraction,
        Multithreading,
    }
}
