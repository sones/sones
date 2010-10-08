/* GraphFS - NServiceDiscovery_Filesystem_Admin
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This class groups all Notification concerning the Automatic Service Discovery
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using sones.Notifications.NotificationTypes;

namespace sones.Notifications.Autodiscovery.NotificationTypes
{
    /// <summary>
    /// This class groups all Filesystem Admin Notifications from the Automatic Service Discovery
    /// </summary>
    public class NServiceDiscovery_StorageEngine : ANotificationType
    {
        public NServiceDiscovery_StorageEngine()
            : base(System.Net.IPAddress.Parse("224.10.10.19"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all StorageEngine Service Announcements (Automatic Service Discovery)."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
