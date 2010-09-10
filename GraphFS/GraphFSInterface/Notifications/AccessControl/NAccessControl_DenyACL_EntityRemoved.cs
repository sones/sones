/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
