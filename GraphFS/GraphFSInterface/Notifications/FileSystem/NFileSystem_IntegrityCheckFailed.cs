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


/* PandoraLib - NIntegrityCheckFailed
 * (c) Stefan Licht, 2009
 * 
 * Notifies about an PandoraFSException_IntegrityCheckFailed exception throwed by InformationHeader.VerifyAndDecrypt(...)
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
    /// Notifies about an PandoraFSException_IntegrityCheckFailed exception throwed by InformationHeader.VerifyAndDecrypt(...)
    /// </summary>
    public class NFileSystem_IntegrityCheckFailed : NFileSystem
    {

        public new class Arguments : INotificationArguments
        {

            public Int32 FailedCopy;
            public Int32 MaxNumberOfCopies;
            public Byte[] SerializedObjectStream;

            #region Constructors

            public Arguments() { }

            public Arguments(Int32 myFailedCopy, Int32 myMaxNumberOfCopies, Byte[] mySerializedObjectStream)
            {
                FailedCopy              = myFailedCopy;
                MaxNumberOfCopies       = myMaxNumberOfCopies;
                SerializedObjectStream  = mySerializedObjectStream;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteObject(FailedCopy);
                _SerializationWriter.WriteObject(MaxNumberOfCopies);
                _SerializationWriter.WriteObject(SerializedObjectStream);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                FailedCopy = (Int32)_SerializationReader.ReadObject();
                MaxNumberOfCopies = (Int32)_SerializationReader.ReadObject();
                SerializedObjectStream = (Byte[])_SerializationReader.ReadObject();
            }

            #endregion
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about an PandoraFSException_IntegrityCheckFailed exception"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
