/* GraphFS - NAccessControl_DefaultRuleChanged
 * (c) Henning Rauch, 2009
 * 
 * Notifies about a changed ACCESSCONTROLSTREAM. The DefaultRule property has been changed.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 * */

#region Usings

using System;
using System.Text;

using sones.Notifications;
using sones.Lib.Serializer;

using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphFS.Notification
{

    /// <summary>
    /// Notifies about a changed ACCESSCONTROLSTREAM. The DefaultRule property has been changed.
    /// </summary>
    public class NAccessControl_DefaultRuleChanged : NAccessControl
    {

        public new class Arguments : INotificationArguments
        {

            public String ObjectLocation;
            public Byte   NewDefaultRule;


            #region Constructors

            public Arguments()
            {
            }

            public Arguments(String myObjectLocation, Byte myNewDefaultRule)
            {
                ObjectLocation = myObjectLocation;
                NewDefaultRule = myNewDefaultRule;
            }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();

                _SerializationWriter.WriteString(ObjectLocation);
                _SerializationWriter.WriteByte(NewDefaultRule);

                return _SerializationWriter.ToArray();

            }

            public void Deserialize(byte[] mySerializedBytes)
            {

                var _SerializationReader = new SerializationReader(mySerializedBytes);

                ObjectLocation = _SerializationReader.ReadString();
                NewDefaultRule = _SerializationReader.ReadOptimizedByte();

            }

            #endregion

        }

        #region ANotificationType

        public override String Description
        {
            get { return "Notifies about a change concerning the DefaultRule Property."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
