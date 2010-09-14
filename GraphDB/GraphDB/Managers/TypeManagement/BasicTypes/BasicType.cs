/* <id name="GraphDB – BasicType Enum" />
 * <copyright file="BasicType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public enum BasicType : short
    {
        Unknown,
        Int64,
        Int32,
        UInt64,
        Double,
        DateTime,
        Boolean,
        String,

        /// <summary>
        /// This might be a Irony NonTerminal as well
        /// </summary>
        NotABasicType,

        /// <summary>
        /// An GraphDB edge containing some objects. 
        /// </summary>
        SetOfDBObjects,

        Reference,
        BackwardEdge,
        Expression,
        Vertex,
        ObjectRevisionID

    }
}
