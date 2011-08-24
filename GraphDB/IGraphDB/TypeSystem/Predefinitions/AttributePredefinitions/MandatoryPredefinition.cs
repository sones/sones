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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// A class that represents a property that has to be mandatory
    /// </summary>
    public sealed class MandatoryPredefinition
    {
        public readonly String MandatoryAttribute;
        public readonly Object DefaultValue;

        /// <summary>
        /// Creates a new instance of MandatoryPredefinition.
        /// </summary>
        /// <param name="myProperty">The property that will be mandatory.</param>
        /// <param name="myDefaultValue">The default value for the mandatory property</param>
        public MandatoryPredefinition(String myProperty, Object myDefaultValue = null) 
        {
            MandatoryAttribute = myProperty;
            DefaultValue = myDefaultValue;
        }
    }
}
