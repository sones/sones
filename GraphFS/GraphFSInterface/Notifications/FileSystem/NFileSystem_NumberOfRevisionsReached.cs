/* GraphLib - NObjectLocationChanged
 * (c) Stefan Licht, 2009
 * 
 * Notifies about any changes for a particular ObjectLocation and their childs.
 * It does not resolve any symlinks!
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

namespace sones.GraphFS.Notification
{
    
    /// <summary>
    /// Notifies about any changes for a particular ObjectLocation and their childs. It does not resolve any symlinks!
    /// </summary>
    public class NFileSystem_NumberOfRevisionsReached : NFileSystem
    {

        public new class Arguments : INotificationArguments
        {
            public String ObjectLocation;
            public String UserMetadataKey;
            public Int32 NumberOfRevisions;

            #region Constructor

            public Arguments() { }

            public Arguments(String myObjectLocation, String myUserMetadataKey, Int32 myNumberOfRevisions)
            {
                ObjectLocation = myObjectLocation;
                UserMetadataKey = myUserMetadataKey;
                NumberOfRevisions = myNumberOfRevisions;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(ObjectLocation);
                _SerializationWriter.WriteString(UserMetadataKey);
                _SerializationWriter.WriteInt32(NumberOfRevisions);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader        = new SerializationReader(mySerializedBytes);
                ObjectLocation                  = _SerializationReader.ReadString();
                UserMetadataKey                 = _SerializationReader.ReadString();
                NumberOfRevisions               = _SerializationReader.ReadInt32();
            }

            public override string ToString()
            {
                return "Number of Revisions for " + ObjectLocation + "." + UserMetadataKey + " reached. (" + NumberOfRevisions + ")";
            }

            #endregion
        
        }

        #region Clientside properties for validate()

        /// <summary>
        /// The ObjectLocation for which the Notification is created. 
        /// </summary>
        public Int32 NumberOfRevisionsThreshold { get; set; }

        public NFileSystem_NumberOfRevisionsReached()
        { }

        public NFileSystem_NumberOfRevisionsReached(Int32 myNumberOfRevisionsThreshold)
        {
            NumberOfRevisionsThreshold = myNumberOfRevisionsThreshold;
        }

        #endregion

        #region ANotificationType Members

        public override Boolean Validate(INotificationArguments myNotificationArguments)
        {

            NFileSystem_NumberOfRevisionsReached.Arguments Args = ((NFileSystem_NumberOfRevisionsReached.Arguments)myNotificationArguments);
            if (Args.NumberOfRevisions > NumberOfRevisionsThreshold)
            {
                return true;
            }
            
            return false;

        }

        public override string Description
        {
            get { return "Notifies about any changes for revisions count."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    }
}
