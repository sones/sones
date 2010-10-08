/* <id name="GraphDB – Graph performance criteria" />
 * <copyright file="GraphPerformanceCriteria.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This enum lists some criteria of a graph</summary>
 */

#region Usings

using System;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{
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
