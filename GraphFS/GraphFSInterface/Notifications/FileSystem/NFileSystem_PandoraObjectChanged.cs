/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* PandoraLib - NPandoraObjectChanged
 * (c) Stefan Licht, 2009
 * 
 * Notifies about any changes for a particular PandoraObject.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications.NotificationTypes;
using sones.Notifications;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphFS.Notification
{

    public class NFileSystem_PandoraObjectChanged : NFileSystem
    {

        public new class Arguments : INotificationArguments
        {
            public String ObjectLocation;
            public Type PandoraObjectType;

            public Arguments() { }

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(ObjectLocation);
                _SerializationWriter.WriteType(PandoraObjectType);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader        = new SerializationReader(mySerializedBytes);
                ObjectLocation                  = _SerializationReader.ReadString();
                PandoraObjectType               = _SerializationReader.ReadTypeOptimized();
            }

            public override string ToString()
            {
                return ObjectLocation;
            }

            #endregion
        }

        #region Clientside properties for validate()

        /// <summary>
        /// The ObjectLocation for which the Notification is created. 
        /// </summary>
        public String ObjectLocation { get; set; }

        public NFileSystem_PandoraObjectChanged()
        { }

        public NFileSystem_PandoraObjectChanged(String myObjectLocation)
        {
            ObjectLocation = myObjectLocation;
        }

        #endregion

        #region ANotificationType

        public override bool Validate(INotificationArguments myNotificationArguments)
        {
            //return ((NPandoraObjectChanged.Arguments)myNotificationArguments).ObjectLocation == ObjectLocation;
            if (ObjectLocation == null)
                return true;
            return ((NFileSystem_PandoraObjectChanged.Arguments)myNotificationArguments).ObjectLocation.StartsWith(ObjectLocation);
        }

        public override string Description
        {
            get { return "Notifies about any changes for a particular PandoraObject"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
