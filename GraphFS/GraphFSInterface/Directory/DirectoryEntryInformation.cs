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

/* PandoraFS - DirectoryEntryInformation
 * (c) Achim Friedland, 2009
 *  
 * An directory entry information is an object which holds all
 * information on an object within an extended directory listing.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections; 
using System.Collections.Generic;


#endregion

namespace sones.GraphFS.InternalObjects
{
  
    /// <summary>
    /// An directory entry information is an object which holds all
    /// information on an object within an extended directory listing.
    /// </summary>

    
    public struct DirectoryEntryInformation
    {

        public String           Name;
        public UInt64           Timestamp;
        public UInt64           Size;
        public HashSet<String>  Streams;

    }

}
