/* GraphLib - Service Discovery Discoverable Service Types
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
