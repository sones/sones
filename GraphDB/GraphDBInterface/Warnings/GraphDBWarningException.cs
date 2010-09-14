/* <id name="GraphDB – GraphDBWarningException" />
 * <copyright file="GraphDBWarningException.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class will "transport" an warning (with the help of an exception) from Irony to the DB.</summary>
 */

using System;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Warnings
{
    /// <summary>
    /// This class will "transport" an warning (with the help of an exception) from Irony to the DB
    /// </summary>
    public class GraphDBWarningException : ApplicationException
    {
        public IWarning GraphDBWarning { get; set; }

        public GraphDBWarningException(IWarning graphDBWarning)
            : base(graphDBWarning.ToString())
        {
            GraphDBWarning = graphDBWarning;
        }
        public GraphDBWarningException(IWarning graphDBWarning, Exception innerException)
            : base(graphDBWarning.ToString(), innerException)
        {
            GraphDBWarning = graphDBWarning;
        }
    }

}
