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

///* PandoraLib - NObjectLocationChanged
// * (c) Stefan Licht, 2009
// * 
// * Notifies about any changes for a particular ObjectLocation and their childs.
// * It does not resolve any symlinks!
// * 
// * Lead programmer:
// *      Stefan Licht
// * 
// * */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//

//using sones.Notifications;
//using sones.Notifications.NotificationTypes;
//using sones.Lib.Serializer;
//using sones.Lib.NewFastSerializer;

//namespace sones.GraphFS.Notification
//{

//    /// <summary>
//    /// Notifies about any changes for a particular ObjectLocation and their childs. It does not resolve any symlinks!
//    /// </summary>
//    public class NFileSystem_ObjectLocationChanged : NFileSystem
//    {

//        public new class Arguments : INotificationArguments
//        {

//            public String _ObjectLocation;
//            public ObjectLocator _ObjectLocator;
//            public String RegisteredObjectLocation;

//            #region Constructor

//            public Arguments() { }

//            public Arguments(String myObjectLocation, ObjectLocator myObjectLocator)
//            {
//                _ObjectLocation = myObjectLocation;
//                _ObjectLocator = myObjectLocator;
//            }

//            public Arguments(String myObjectLocation, ObjectLocator myObjectLocator, String myRegisteredObjectLocation)
//                : this(myObjectLocation, myObjectLocator)
//            {
//                RegisteredObjectLocation = myRegisteredObjectLocation;
//            }

//            #endregion

//            #region INotificationArguments Members

//            public byte[] Serialize()
//            {
//                var _SerializationWriter = new SerializationWriter();
//                _SerializationWriter.WriteObject(_ObjectLocation);
//                _SerializationWriter.WriteObject(_ObjectLocator);
//                _SerializationWriter.WriteObject(RegisteredObjectLocation);

//                return _SerializationWriter.ToArray();
//            }

//            public void Deserialize(byte[] mySerializedBytes)
//            {
//                var _SerializationReader = new SerializationReader(mySerializedBytes);
//                _ObjectLocation = (String)_SerializationReader.ReadObject();
//                _ObjectLocator = (ObjectLocator)_SerializationReader.ReadObject();
//                RegisteredObjectLocation = (String)_SerializationReader.ReadObject();
//            }

//            #endregion

//        }

//        #region Clientside properties for validate()

//        /// <summary>
//        /// The ObjectLocation for which the Notification is created. 
//        /// </summary>
//        public String ObjectLocation { get; set; }

//        public NFileSystem_ObjectLocationChanged()
//        { }

//        public NFileSystem_ObjectLocationChanged(String myObjectLocation)
//        {
//            ObjectLocation = myObjectLocation;
//        }

//        #endregion

//        #region ANotificationType Members

//        public override Boolean Validate(INotificationArguments myNotificationArguments)
//        {

//            NFileSystem_ObjectLocationChanged.Arguments Args = ((NFileSystem_ObjectLocationChanged.Arguments)myNotificationArguments);
//            String changedObjectLocation = Args._ObjectLocation;
//            String registeredForObjectLocation = ObjectLocation;
//            if (changedObjectLocation.StartsWith(registeredForObjectLocation))
//            {
//                Args.RegisteredObjectLocation = registeredForObjectLocation;
//                return true;
//            }

//            return false;

//        }

//        public override string Description
//        {
//            get { return "Notifies about any changes for a particular ObjectLocation and their childs."; }
//        }

//        public override INotificationArguments GetEmptyArgumentInstance()
//        {
//            return new Arguments();
//        }

//        #endregion

//    }

//}
