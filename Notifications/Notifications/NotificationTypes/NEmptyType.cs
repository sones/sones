using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.NotificationTypes
{
    public class NEmptyType : ANotificationType
    {

        public NEmptyType()
            : base(System.Net.IPAddress.Parse("224.10.10.1"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all emtpy test Notifications."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
