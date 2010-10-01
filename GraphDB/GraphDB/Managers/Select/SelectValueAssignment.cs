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

/*
 * SelectValueAssignment
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Managers.Select
{

    /// <summary>
    /// A value assignment in select. PBI 527
    /// </summary>
    public class SelectValueAssignment
    {
        
        #region ValueAssignmentTypes

        /// <summary>
        /// The type of the value assignment.
        /// </summary>
        public enum ValueAssignmentTypes
        {
            Always,
            IfNotExists
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of the value assignment.
        /// </summary>
        public ValueAssignmentTypes ValueAssignmentType { get; private set; }

        /// <summary>
        /// The value of the assignment. Currently only ValueDefinitions are allowed but at some time this could be anything, even AExpressionDefinition
        /// </summary>
        public ATermDefinition TermDefinition { get; internal set; }

        #endregion

        #region Ctor

        public SelectValueAssignment(ValueAssignmentTypes myValueAssignmentType, ValueDefinition myValueDefinition)
        {
            ValueAssignmentType = myValueAssignmentType;
            TermDefinition = myValueDefinition;
        }

        #endregion

    }

}
