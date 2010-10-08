/* <id name="GraphDB – Node interface" />
 * <copyright file="IExpressionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Interface for the expression edge.</summary>
 */

#region Usings

using System;
using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{

    /// <summary>
    /// Interface for the expression edge.
    /// </summary>

    public interface IExpressionEdge
    {
        EdgeKey Direction { get; }
        ObjectUUID Destination { get; }
    }
}
