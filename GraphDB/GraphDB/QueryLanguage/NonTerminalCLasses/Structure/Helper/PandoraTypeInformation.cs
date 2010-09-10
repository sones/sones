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

/* <id name="PandoraDB – PandoraTypeInformation" />
 * <copyright file="PandoraTypeInformation.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Carries ID Informations.</summary>
 */

#region usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// Carries ID Informations
    /// </summary>
    public class PandoraTypeInformation
    {
        #region Data

        private String _Reference;
        private String _lastAttributeName;
        private IDNode _IDNode;

        #endregion

        #region Constructor

        public PandoraTypeInformation(IDNode aIDNode)
        {
            if (aIDNode.LastAttribute != null)
            {
                _lastAttributeName = aIDNode.LastAttribute.Name;
            }
            else
            {
                _lastAttributeName = null;
            }

            _Reference = aIDNode.Reference.Item1;
            _IDNode = aIDNode;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// The reference... sth like U or User
        /// </summary>
        public String Reference { get { return _Reference; } }

        /// <summary>
        /// 
        /// </summary>
        public String LastAttributeName { get { return _lastAttributeName; } }
        public IDNode aIDNode { get { return _IDNode; } }

        #endregion
    }
}
