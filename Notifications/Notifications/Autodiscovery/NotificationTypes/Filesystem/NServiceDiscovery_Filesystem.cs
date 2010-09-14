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
    public class NServiceDiscovery_Filesystem : ANotificationType
    {
        public NServiceDiscovery_Filesystem()
            : base(System.Net.IPAddress.Parse("224.10.10.16"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all Filesystem Service Announcements (Automatic Service Discovery)."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
