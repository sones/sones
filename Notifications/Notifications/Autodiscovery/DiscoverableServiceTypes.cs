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

/* PandoraLib - Service Discovery Discoverable Service Types
 * (c) Daniel Kirstenpfad, 2009
 * 
 * An enum type to list all possible discoverable services
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings
using System.Net;
using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Notifications.Autodiscovery
{
    /// <summary>
    /// An enum type to list all possible discoverable services
    /// </summary>
    
    public enum DiscoverableServiceType : long
    {
        Filesystem                      = 3758754319, // 224.10.10.15
        Database                        = 3758754320, // 224.10.10.16
        StorageEngine                   = 3758754321, // 224.10.10.17
    }

}
