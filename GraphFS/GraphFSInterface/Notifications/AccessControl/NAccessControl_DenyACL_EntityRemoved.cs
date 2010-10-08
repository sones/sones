/* GraphFS - NAccessControl_DenyACL_EntityRemoved
 * (c) Henning Rauch, 2009
 * 
 * Notifies about a changed ACCESSCONTROLSTREAM. An Entity has been removed from the DenyACL.
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
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.Notification
{

    /// <summary>
    /// Notifies about a changed ACCESSCONTROLSTREAM. An Entity has been removed from the DenyACL.
    /// </summary>
    public class NAccessControl_DenyACL_EntityRemoved : NAccessControl
    {

        public new class Arguments : INotificationArguments
        {

            #region Data

            public String ObjectLocation;
            public UUID   RightUUID;
            public UUID   EntitiyUUID;

            #endregion

            #region Constructors

            public Arguments()
            {
            }

            public Arguments(String myObjectLocation, UUID myRightUUID, UUID myEntitiyUUID)
            {
                ObjectLocation  = myObjectLocation;
                RightUUID       = myRightUUID;
                EntitiyUUID     = myEntitiyUUID;
            }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();

                _SerializationWriter.WriteString(ObjectLocation);
                RightUUID.Serialize(ref _SerializationWriter);
                EntitiyUUID.Serialize(ref _SerializationWriter);
               
                return _SerializationWriter.ToArray();

            }

            public void Deserialize(byte[] mySerializedBytes)
            {

                var _SerializationReader = new SerializationReader(mySerializedBytes);

                ObjectLocation = _SerializationReader.ReadString();
                RightUUID       = new UUID(_SerializationReader.ReadByteArray());
                EntitiyUUID     = new UUID(_SerializationReader.ReadByteArray());
                
            }

            #endregion

        }

        #region ANotificationType

        public override String Description
        {
            get { return "Notifies about a removed entity from DenyACL."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
