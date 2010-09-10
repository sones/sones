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

/* <id name="GraphLib – Autocompletion" />
 * <copyright file="GraphTypeAC.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
using sones.GraphDS.API.CSharp;

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

        public override List<String> Complete(AGraphDSSharp myGraphDSSharp, ref String CurrentPath, string CurrentStringLiteral)
        {

            var possibleGraphTypes = new List<String>();

            if (myGraphDSSharp.IGraphDBSession != null)
            {

                using (var transaction = myGraphDSSharp.IGraphDBSession.BeginTransaction())
                {

                    foreach (var _GraphType in ((DBContext)transaction.GetDBContext()).DBTypeManager.GetAllTypes())
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
