/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id name="PandoraLib – Autocompletion" />
 * <copyright file="PandoraTypeAC.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements the autocompletion
 * for actual PandoraTypes.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// This class implements the autocompletion
    /// for actual PandoraTypes.
    /// </summary>

    public class StringLiteralPandoraType : ADBCLIAutocompletions
    {

        #region properties

        public override String Name { get { return "stringLiteralPandoraType"; } }

        #endregion

        #region completion method

        public override List<String> Complete(ref object PVFSObject, ref object PDBObject, ref String CurrentPath, string CurrentStringLiteral)
        {

            var myPandoraDB = PDBObject as GraphDBSession;
            var possiblePandoraTypes = new List<String>();

            if (myPandoraDB != null)
            {
                foreach (var _PandoraType in myPandoraDB.GetDBContext().DBTypeManager.GetAllTypes())
                {
                    if (_PandoraType.Name.StartsWith(CurrentStringLiteral))
                    {
                        possiblePandoraTypes.Add(_PandoraType.Name);
                    }
                }

            }

            return possiblePandoraTypes;
            
        }

        #endregion

    }

}
