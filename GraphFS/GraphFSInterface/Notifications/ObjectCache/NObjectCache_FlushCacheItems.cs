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
    public class NObjectCache_FlushCacheItems : NObjectCache
    {

        public new class Arguments : INotificationArguments
        {

            public String[] CachedItemKeysToFlush;

            #region Constructor

            public Arguments() { }

            public Arguments(String[] myCachedItemKeysToFlush)
            {
                if (myCachedItemKeysToFlush.Length > 20)
                {
                    CachedItemKeysToFlush = new String[20];
                    Array.Copy(myCachedItemKeysToFlush, CachedItemKeysToFlush, 19);
                    CachedItemKeysToFlush[19] = "... " + (myCachedItemKeysToFlush.Length - 19) + " more";
                }
                else
                {
                    CachedItemKeysToFlush = myCachedItemKeysToFlush;
                }
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteObject(CachedItemKeysToFlush);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                CachedItemKeysToFlush = (String[])_SerializationReader.ReadObject();
            }

            public override String ToString()
            {
                StringBuilder retVal = new StringBuilder();
                foreach (String key in CachedItemKeysToFlush)
                    retVal.AppendLine(key);

                return retVal.ToString();
            }

            #endregion
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about new items flushing"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
