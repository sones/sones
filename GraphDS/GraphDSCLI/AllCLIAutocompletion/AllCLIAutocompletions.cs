/* <id name=”GraphLib – abstract AllCLIAutocompletions class” />
 * <copyright file=”AllCLIAutocompletions.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The abstract class for all autocompletions of 
 * the GraphCLI.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    public abstract class AllCLIAutocompletions
    {

        /// <summary>
        /// The abstract class for all autocompletions of 
        /// the GraphCLI.
        /// </summary>

        #region Data        

        #endregion

        #region Properties

        public abstract String Name { get; }

        #endregion

        #region (public) Methods

        #region completion Method

        public abstract List<String> Complete(AGraphDSSharp myGraphDSSharp, ref String CurrentPath, string CurrentStringLiteral);

        #endregion

        #endregion

    }
}
