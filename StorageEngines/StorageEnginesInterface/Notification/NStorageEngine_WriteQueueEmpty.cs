/* GraphLib - NStorageEngine_WriteQueueEmpty
 * (c) Stefan Licht, 2009
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications;
using sones.Notifications.NotificationTypes;

namespace sones.StorageEngines.Notification
{

    /// <summary>
    /// Notifies about an WriteQueue which is empty.
    /// </summary>

    public class NStorageEngine_WriteQueueEmpty : NStorageEngine
    {

        public new class Arguments : INotificationArguments
        {
            public String StorageLocation;

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                return new Byte[0];
            }

            public void Deserialize(byte[] mySerializedBytes)
            { }

            #endregion
        }

        #region INotificationType Members

        public override string Description
        {
            get { return "Notifies about an WriteQueue which is empty"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
