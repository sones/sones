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
 * for actual GraphTypes.</summary>
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
    /// for actual GraphTypes.
    /// </summary>

    public class StringLiteralGraphType : ADBCLIAutocompletions
    {

        #region properties

        public override String Name { get { return "stringLiteralGraphType"; } }

        #endregion

        #region completion method

        public override List<String> Complete(ref object PVFSObject, ref object PDBObject, ref String CurrentPath, string CurrentStringLiteral)
        {

            var myGraphDB = PDBObject as GraphDBSession;
            var possibleGraphTypes = new List<String>();

            if (myGraphDB != null)
            {

                using (var transaction = myGraphDB.BeginTransaction())
                {

                    foreach (var _GraphType in transaction.GetDBContext().DBTypeManager.GetAllTypes())
                    {
                        if (_GraphType.Name.StartsWith(CurrentStringLiteral))
                        {
                            possibleGraphTypes.Add(_GraphType.Name);
                        }
                    }

                }

            }

            return possibleGraphTypes;
            
        }

        #endregion

    }

}
