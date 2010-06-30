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


/* PandoraFS - EntityStatus
 * Achim Friedland, 2009
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

namespace sones.GraphFS.InternalObjects
{

    [Flags]
    public enum EntityStatus : ushort
    {

        // This entity is disbaled and should act like an
        // entity without any rights.
        DISABLED,

        // This entity must not be included within the
        // membership hashset of any entity.
        SEALED

    }

}
