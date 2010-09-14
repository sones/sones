/* GraphLib - NMissingCacheItem
 * (c) Stefan Licht, 2009
 * 
 * Notifies about a not found CacheEntry for a ObjectLocation
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

    /// <summary>
    /// Notifies about a not found CacheEntry for a ObjectLocation
    /// </summary>
    public class NObjectCache_NewCacheItem : NObjectCache
    {

        public new class Arguments : INotificationArguments
        {
            
            public String ObjectLocation;
            public Type TypeOfCacheEntry;

            #region Constructor

            public Arguments() { }

            public Arguments(Type myTypeOfCacheEntry)
            {
                TypeOfCacheEntry = myTypeOfCacheEntry;
            }

            public Arguments(String myObjectLocation, Type myTypeOfCacheEntry) : this(myTypeOfCacheEntry)
            {
                ObjectLocation = myObjectLocation;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(ObjectLocation);
                _SerializationWriter.WriteType(TypeOfCacheEntry);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader    = new SerializationReader(mySerializedBytes);
                ObjectLocation              = _SerializationReader.ReadString();
                TypeOfCacheEntry            = _SerializationReader.ReadTypeOptimized();
            }

            #endregion
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about a new CacheEntry for a ObjectLocation"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
