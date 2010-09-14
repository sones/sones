/*
 * NGraphDS
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Net;
using System.Text;

using sones.Notifications;
using sones.Notifications.NotificationTypes;

#endregion


namespace sones.GraphDS.API.CSharp.Notifications
{

    /// <summary>
    /// Groups all notifications of a GraphDS instance
    /// </summary>

    public class NGraphDS : ANotificationType
    {

        public NGraphDS()
            : base(IPAddress.Parse("224.10.10.30"), 5000)
        { }

        public override string Description
        {
            get { return "Groups all notifications of a GraphDS instance."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }

    }

}
