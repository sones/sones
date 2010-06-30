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


/* PandoraFS - AccessFlagsType
 * Achim Friedland, 2008
 * 
 * A structure for storing the AccessFlags of an object
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// A structure for storing the AccessFlags of an object
    /// </summary>

    
    [Flags]
    public enum AccessFlagsType : ushort
    {

        NOTHING               = 0x0000,     // 0000 0000

        CREATE                = 0x0,
        READ                  = 0x0001,     // 0000 0001
        WRITE                 = 0x0002,     // 0000 0010
        APPEND                = 0x0004,     // 0000 0100
        DELETE                = 0x0,
        ERASE                 = 0x0,

        EXECUTE               = 0x0008,     // 0000 1000

        CHANGE_STORAGEUUIDS   = 0x0,
        CHANGE_SAFETY         = 0x0010,     // 0001 0000
        CHANGE_SECURITY       = 0x0020,     // 0010 0000

        VIEW_OTHERACLS        = 0x0000,
        CHANGE_OTHERACLS      = 0x0040,     // 0100 0000
        CHANGE_OWNACL         = 0x0040,     // 0100 0000

//        OWNER_WRITE           = 0x0080,     // 1000 0000

    }

}
