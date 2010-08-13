///* GraphLib - NObjectLocatorChanged
// * (c) Stefan Licht, 2009
// * 
// * Notifies about any changes for a particular ObjectLocator.
// * 
// * Lead programmer:
// *      Stefan Licht
// * 
// * */

//using System;
//using System.Text;
//using System.Linq;
//using System.Collections.Generic;
//using sones.Notifications;
//using sones.Lib.NewFastSerializer;

//namespace sones.GraphFS.Notification
//{
//    public class NFileSystem_ObjectLocatorChanged : NFileSystem
//    {

//        public new class Arguments : INotificationArguments
//        {

//            public String ObjectLocation;
//            public ObjectLocator ObjectLocator;

//            public Arguments() { }

//            #region INotificationArguments Members

//            public byte[] Serialize()
//            {
//                var _SerializationWriter = new SerializationWriter();
//                _SerializationWriter.WriteObject(ObjectLocation);
//                _SerializationWriter.WriteObject(ObjectLocator);

//                return _SerializationWriter.ToArray();
//            }

//            public void Deserialize(byte[] mySerializedBytes)
//            {
//                var _SerializationReader = new SerializationReader(mySerializedBytes);
//                ObjectLocation = (String)_SerializationReader.ReadObject();
//                ObjectLocator = (ObjectLocator)_SerializationReader.ReadObject();
//            }

//            #endregion
        
//        }

//        #region Clientside properties for validate()

//        /// <summary>
//        /// The ObjectLocation for which the Notification is created. 
//        /// </summary>
//        public String ObjectLocation { get; set; }

//        public NFileSystem_ObjectLocatorChanged()
//        { }

//        public NFileSystem_ObjectLocatorChanged(String myObjectLocation)
//        {
//            ObjectLocation = myObjectLocation;
//        }

//        #endregion

//        #region ANotificationType

//        public override bool Validate(INotificationArguments myNotificationArguments)
//        {
//            return ((NFileSystem_ObjectLocatorChanged.Arguments)myNotificationArguments).ObjectLocation == ObjectLocation;
//        }

//        public override string Description
//        {
//            get { return "Notifies about any changes for a particular ObjectLocator"; }
//        }

//        public override INotificationArguments GetEmptyArgumentInstance()
//        {
//            return new Arguments();
//        }

//        #endregion

//    }
//}
