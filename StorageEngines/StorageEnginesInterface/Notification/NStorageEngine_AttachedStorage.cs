/* GraphLib - NStorageEngine_AttachedStorage
 * (c) Stefan Licht, 2009
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
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.StorageEngines.Notification
{

    /// <summary>
    /// Notifies about a used StorageEngine.
    /// </summary>

    public class NStorageEngine_AttachedStorage : NStorageEngine
    {

        public new class Arguments : INotificationArguments
        {

            public List<String> StorageURIs;
            public String StorageType;
            public UInt64 StorageSize;

            public Arguments() { }

            public Arguments(List<String> myStorageLocations, String myStorageType, UInt64 myStorageSize)
            {
                StorageURIs = myStorageLocations;
                StorageType = myStorageType;
                StorageSize = myStorageSize;
            }

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                if (StorageURIs == null)
                    _SerializationWriter.WriteUInt32(0);
                else
                    _SerializationWriter.WriteUInt32((UInt32)StorageURIs.Count);

                foreach (String uri in StorageURIs)
                    _SerializationWriter.WriteString(uri);

                _SerializationWriter.WriteString(StorageType);
                _SerializationWriter.WriteUInt64(StorageSize);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);

                StorageURIs = new List<String>();
                UInt32 numberOfStorageURIs = _SerializationReader.ReadUInt32();

                for (Int32 i = 0; i < numberOfStorageURIs; i++)
                    StorageURIs.Add(_SerializationReader.ReadString());

                StorageType = _SerializationReader.ReadString();
                StorageSize = _SerializationReader.ReadUInt64();

            }

            public override string ToString()
            {
                String retVal = String.Concat("[FreeStorage] ", " Type: ", StorageType, " StorageSize(MB): ", (Math.Round((decimal)StorageSize / 1024 / 1024, 3)),
                    " uris: ", StorageURIs.Count);

                if (StorageURIs.Count > 0)
                    retVal += String.Concat(" First uri: " + ((StorageURIs.Count > 1) ? StorageURIs[1] : StorageURIs[0]));

                return retVal;
            }

            #endregion

        }

        #region INotificationType Members

        public override string Description
        {
            get { return "Notifies about a used StorageEngine"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }

}
