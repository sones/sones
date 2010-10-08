/* GraphLib - NDatabaseMisc
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Notifies about an Misc. Database Event
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using sones.Notifications;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Notification.NotificationTypes.Database
{
    /// <summary>
    /// Notifies about an Misc. Database Event
    /// </summary>
    public class NDatabaseMisc : NotificationGroupDatabase
    {

        public new class Arguments : INotificationArguments
        {
            public String Message;

            #region Constructors

            public Arguments() { }

            public Arguments(String MyMessage)
            {
                Message = MyMessage;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(Message);
               
                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                Message = _SerializationReader.ReadString();
            }

            #endregion
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about Miscelleanous Database Events"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
