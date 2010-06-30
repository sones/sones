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


/* <id name="sones GraphDB – chain data" />
 * <copyright file="ChainData.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class is used for aggregating type/attribute chain data.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This class is used for aggregating type/attribute chain data.
    /// </summary>
    class ChainData
    {
        #region Data

        GraphDBType _pandoraType = null;
        List<String> _relevantGuids = null;

        #endregion

        #region constructor

        public ChainData(GraphDBType pandoraType, List<String> relevantGuids)
        {
            _pandoraType = pandoraType;
            _relevantGuids = relevantGuids;
        }

        #endregion

        #region accessors

        public List<String> RelevantGuids { get { return _relevantGuids; } }
        public GraphDBType PandoraTypeOfGuids { get { return _pandoraType; } }

        #endregion
    }
}
