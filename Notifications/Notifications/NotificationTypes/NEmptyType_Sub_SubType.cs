/* GraphLib - NEmptyType
 * (c) Stefan Licht, 2009
 * 
 * This is an empty Notification without any arguments and name.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.NotificationTypes
{
    public class NEmptyType_Sub_SubType : NEmptyType_SubType
    {

        public NEmptyType_Sub_SubType() { }

        public new class Arguments : INotificationArguments
        {
            #region INotificationArguments Members

            public byte[] Serialize()
            { 
                return new Byte[0];
            }

            public void Deserialize(byte[] mySerializedBytes)
            { }

            public new String ToString()
            {
                return String.Empty;
            }

            #endregion
        }

        #region ANotificationType Members

        public override string Description
        {
            get { return "Just an empty test notification"; }
        }

        //public override string Name
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }
    }
}
