/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name=”PandoraLib – abstract AllCLIAutocompletions class” />
 * <copyright file=”AllCLIAutocompletions.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The abstract class for all autocompletions of 
 * the PandoraCLI.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

#endregion

namespace sones.Lib.CLI
{

    public abstract class AllCLIAutocompletions
    {

        /// <summary>
        /// The abstract class for all autocompletions of 
        /// the PandoraCLI.
        /// </summary>

        #region Data        

        #endregion

        #region Properties

        public abstract String Name { get; }

        #endregion

        #region (public) Methods

        #region completion Method

        public abstract List<String> Complete(ref object PVFSObject, ref object PDBObject, ref String CurrentPath, string CurrentStringLiteral);

        #endregion

        #endregion

    }
}
