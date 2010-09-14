/* GraphFS - NAccessControl_AllowACL_EntityAdded
 * (c) Henning Rauch, 2009
 * 
 * Notifies about a changed ACCESSCONTROLSTREAM. An Entity has been added to the AllowACL.
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
    /// Notifies about a changed ACCESSCONTROLSTREAM. An Entity has been added to the AllowACL.
    /// </summary>
    public class NAccessControl_AllowACL_EntityAdded : NAccessControl
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

                var writer = new SerializationWriter();

                writer.WriteString(ObjectLocation);
                RightUUID.Serialize(ref writer);
                EntitiyUUID.Serialize(ref writer);
               
                return writer.ToArray();

            }

            public void Deserialize(byte[] mySerializedBytes)
            {

                var reader = new SerializationReader(mySerializedBytes);

                ObjectLocation = reader.ReadString();
                RightUUID       = new UUID(reader.ReadByteArray());
                EntitiyUUID     = new UUID(reader.ReadByteArray());
                
            }

            #endregion

        }

        #region ANotificationType

        public override String Description
        {
            get { return "Notifies about an added entity to AllowACL"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }

}
