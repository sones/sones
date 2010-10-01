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

/* GraphLib - NotificationMessage
 * (c) Stefan Licht, 2009
 * 
 * This is the message, created and send from the dispatcher 
 * to all subscribed clients.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Notifications.NotificationTypes;

namespace sones.Notifications.Messages
{
    
    public class NotificationMessage : IComparable<NotificationMessage>
    {

        #region fields

        private NotificationPriority _Priority;
        private Guid _Class;
        private Int64 _Created_TimeStamp;
        private INotificationArguments _Arguments;
        private String _NotificationType;
        private List<String> _HandledDispatchers;

        #endregion

        #region Properties

        public NotificationPriority Priority      // the Priority of the packet, really just a byte
        {
            get
            {
                return _Priority;
            }
        }

        public Guid Class                  // the class of this message
        {
            get
            {
                return _Class;
            }
        }

        public Int64 Created_TimeStamp        // the timestamp (ticks) when this message was sent by the sender
        {
            get
            {
                return _Created_TimeStamp;
            }
        }

        public INotificationArguments Arguments // the actual content of the message
        {
            get
            {
                return _Arguments;
            }
        }

        public String NotificationType
        {
            get
            {
                return _NotificationType;
            }
        }

        public SenderInfo SenderInfo { get; set; }

        /// <summary>
        /// Store all Dispatcher Guids to avoid cycles
        /// </summary>
        public List<String> HandledDispatchers
        {
            get { return _HandledDispatchers; }
        }

        public Int32 Retries { get; set; }

        #endregion

        #region Constructors

        public NotificationMessage(Byte[] mySerializedData, INotificationArguments myArguments)
        {
            Deserialize(mySerializedData, myArguments);
            Retries = 0;
        }

        private NotificationMessage(NotificationPriority Priority, Int64 Created_TimeStamp, INotificationArguments Arguments)
        {
            _Priority           = Priority;
            _Created_TimeStamp  = Created_TimeStamp;
            _Arguments          = Arguments;
            _Class              = Guid.NewGuid();
            _HandledDispatchers = new List<string>();
            Retries             = 0;
        }

        public NotificationMessage(NotificationPriority Priority, Int64 Created_TimeStamp, INotificationArguments Arguments, Type myNotificationType)
            : this(Priority, Created_TimeStamp, Arguments)
        {
            _NotificationType = myNotificationType.FullBaseName<ANotificationType>();
        }

        #endregion

        #region (De)Serialize

        public Byte[] Serialize()
        {
            SerializationWriter writer = new SerializationWriter();
            writer.WriteObject(_Class);
            writer.WriteString(_NotificationType);
            writer.WriteInt64(_Created_TimeStamp);
            writer.WriteByte((Byte)_Priority);
            writer.Write(_Arguments.Serialize());

            writer.WriteUInt32((UInt32)_HandledDispatchers.Count);

            foreach (String str in _HandledDispatchers)
                writer.WriteString(str);
            
            return writer.ToArray();
        }

        private void Deserialize(Byte[] mySerializedBytes, INotificationArguments myArguments)
        {
            SerializationReader reader = new SerializationReader(mySerializedBytes);

            _Class = (Guid)reader.ReadObject();
            _NotificationType = reader.ReadString();
            _Created_TimeStamp = reader.ReadInt64();
            _Priority = (NotificationPriority)(Byte)reader.ReadOptimizedByte();
            _Arguments = myArguments;
            _Arguments.Deserialize(reader.ReadByteArray());

            UInt32 _HandledDispatchersCount = reader.ReadUInt32();

            _HandledDispatchers = new List<string>();

            for(UInt32 i = 0; i < _HandledDispatchersCount; i++)
                _HandledDispatchers.Add(reader.ReadString());
        }

        #endregion

        #region IComparable<NotificationMessage> Members

        public int CompareTo(NotificationMessage other)
        {
            if (_Class.Equals(other.Class))
                return 0;
            /*
            // higher priority and newer
            if (_Priority > other._Priority && _Created_TimeStamp > other.Created_TimeStamp)
                return -1;

            // highest priority and older
            else if (_Priority > PriorityTypes.Severe && _Created_TimeStamp < other.Created_TimeStamp)
                return -1;
            */
            // highest priority and older
            else if (_Created_TimeStamp < other.Created_TimeStamp)
                return -1;
            
            else 
                return 1;
            
            /*
            else if (_Priority == other._Priority)
            {
                if (_Created_TimeStamp > other.Created_TimeStamp)
                    return -1;
                else 
                    return 1;
            }

            else if (_Priority > other._Priority) return -1;
            return 0;
            */
        }

        #endregion

        public void HandleByDispatcher(String myDispatcherGuid)
        {
            _HandledDispatchers.Add(myDispatcherGuid);
        }

        public Boolean IsHandledByDispatcher(String myDispatcherGuid)
        {
            return _HandledDispatchers.Contains(myDispatcherGuid);
        }

        public NotificationMessage Copy()
        {
            NotificationMessage message = new NotificationMessage(_Priority, _Created_TimeStamp, _Arguments);

            message._NotificationType = NotificationType;

            return message;
        }

    }
}
