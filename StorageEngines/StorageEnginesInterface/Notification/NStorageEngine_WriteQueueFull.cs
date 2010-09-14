/* GraphLib - NStorageEngine_WriteQueueFull
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
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.StorageEngines.Notification
{

    /// <summary>
    /// Notifies about an WriteQueue which is full.
    /// </summary>

    public class NStorageEngine_WriteQueueFull : NStorageEngine
    {

        public new class Arguments : INotificationArguments
        {

            public UInt32 NumberOfQueueEntries;

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteUInt32(NumberOfQueueEntries);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                NumberOfQueueEntries = _SerializationReader.ReadUInt32();
            }

            #endregion

        }


        #region INotificationType Members

        public override string Description
        {
            get { return "Notifies about an WriteQueue which is full"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    
    }

}
