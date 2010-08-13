/* <id name="GraphDB – ResultType Enum" />
 * <copyright file="ResultTypeEnum.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class carries information of errors.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.Structures
{
    
    public enum ResultType
    {
        Failed,
        PartialSuccessful,
        Successful,
    }

}