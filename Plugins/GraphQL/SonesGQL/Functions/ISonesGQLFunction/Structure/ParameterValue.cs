/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISonesGQLFunction.Structure
{
    /// <summary>
    /// A Function parameter containing the parameter name and the type.
    /// This is used by the parameter definition in the function implementation.
    /// </summary>
    public struct ParameterValue
    {

        public readonly String Name;
        public readonly Object Value;
        public readonly Boolean VariableNumOfParams;

        /// <summary>
        /// A single parameter
        /// </summary>
        public ParameterValue(String myParameterName, Object myParameterValue)
        {
            Name = myParameterName;
            Value = myParameterValue;
            VariableNumOfParams = false;
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myVariableNumOfParams">True if this parameter occurs 1 or more time. This have to be the last parameter!</param>
        public ParameterValue(String myParameterName, Object myParameterValue, Boolean myVariableNumOfParams)
        {
            Name = myParameterName;
            Value = myParameterValue;
            VariableNumOfParams = myVariableNumOfParams;
        }

    }
}
