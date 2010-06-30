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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphFS.Notification
{
    public class NException_UnhandledExceptionInsideAThread : NException
    {

        public new class Arguments : INotificationArguments
        {

            public String ThrowedException;

            #region Constructors

            public Arguments() { }

            public Arguments(Exception myThrowedException)
            {
                ThrowedException = myThrowedException.Message + Environment.NewLine + myThrowedException.StackTrace;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteObject(ThrowedException);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                ThrowedException = (String)_SerializationReader.ReadObject();
            }

            #endregion
        
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about any exception which is unhandled and throwed in any spawned thread."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    
    }

}
