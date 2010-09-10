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
 * AccessModeTypes
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// The different access modes (read/write, read-only, ...)
    /// of a file system.
    /// </summary>

    

    public enum AccessModeTypes
    {
        //
        // The filesystem should be opened for reading and writing
        rw = 0,
        //
        // The filesystem will be opened only for appending data
        ap = 1,
        //
        // The filesystem will be opened only for reading
        ro = 2,
        //
        // ---------------------------------------------
        //
        // The filesystem should be opened only for reading and the metadata for reading and writing
        metarw = 3,
        //
        // The filesystem should be opened only for reading and the metadata only for appending
        metaap = 4,
    }

}
