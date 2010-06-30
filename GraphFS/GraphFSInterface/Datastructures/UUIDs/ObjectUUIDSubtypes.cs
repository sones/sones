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


/* PandoraFS - ObjectUUIDSubtypes
 * Achim Friedland, 2008 - 2009
 * 
 * The ObjectUUID is a common prefix for all UUIDs within the INodes,
 * ObjectLocators and ObjectStreams located at a specific ObjectLocation.
 * The ObjectUUIDSubtypes are an addition to this UUID prefix for
 * distinguishing the different types of objects and structures within
 * such a object group.
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
    /// The ObjectUUID is a common prefix for all UUIDs within the INodes,
    /// ObjectLocators and ObjectStreams located at a specific ObjectLocation.
    /// The ObjectUUIDSubtypes are an addition to this UUID prefix for
    /// distinguishing the different types of objects and structures within
    /// such a object group.
    /// </summary>

    [Flags]

    

    public enum ObjectUUIDSubtypes : byte
    {

        INODE                   = 3,       // 0000 0011
        OBJECTLOCATOR           = 15,      // 0000 1111
        OBJECTSTREAM            = 63,      // 0011 1111

    }

}
