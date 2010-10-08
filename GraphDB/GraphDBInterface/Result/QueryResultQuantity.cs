/* <id name="GraphDB – QueryResultQuantity Enum" />
 * <copyright file="QueryResultQuantityEnum.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Gives a hint about how many selectionListResult elements have been delivered.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDBInterface.Result
{
    /// <summary>
    /// Gives a hint about how many selectionListResult elements have been delivered.
    /// </summary>
    public enum QueryResultQuantity
    {
        None = 0x01,
        Single = 0x02,
        Multiple = 0x04,
    }
}
