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

/* PandoraLib - NStorageEngine_FreeStorage
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
    /// Notifies about a free and not used StorageEngine.
    /// </summary>

    public class NStorageEngine_FreeStorage : NStorageEngine
    {

        public new class Arguments : INotificationArguments
        {

            //public String StorageURI;
            public List<String> StorageURIs;
            public String StorageType;
            public UInt64 StorageSize;
            //public Byte[] StorageUUID;

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
                    _SerializationWriter.WriteObject((Int32)0);
                else
                    _SerializationWriter.WriteObject((Int32)StorageURIs.Count);

                foreach(String uri in StorageURIs)
                    _SerializationWriter.WriteObject(uri);

                _SerializationWriter.WriteObject(StorageType);
                _SerializationWriter.WriteObject(StorageSize);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);

                StorageURIs = new List<String>();
                Int32 numberOfStorageURIs = (Int32)_SerializationReader.ReadObject();

                for(Int32 i=0; i<numberOfStorageURIs; i++)
                    StorageURIs.Add((String)_SerializationReader.ReadObject());

                StorageType = (String)_SerializationReader.ReadObject();
                StorageSize = (UInt64)_SerializationReader.ReadObject();

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
            get { return "Notifies about a free and not used StorageEngine"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion


    }
}
