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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// The kind of a type (Single, List)
    /// </summary>

    public enum KindsOfType
    {

        SingleNoneReference,
        SingleReference,
        ListOfNoneReferences,
        SetOfNoneReferences,
        SetOfReferences,
        //SettingAttribute,
        SpecialAttribute,

        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownSet,
        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownList,
        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownSingle
    }

}
