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


/* PandoraLib - NotificationBridgeConnection
 * (c) Stefan Licht, 2009
 * 
 * This class is the generic class for the UDPSocketConnection pattern.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Specialized;
using sones.Lib.NewFastSerializer;
using sones.Notifications.Exceptions;
using sones.Notifications.Messages;
using sones.Notifications.NotificationTypes;
using sones.Networking.UDPSocket;
using sones.Notifications.Exceptions;

namespace sones.Notifications
{
    /// <summary>
    /// This class is the generic class for the <paramref name="UDPSocketConnection"/> pattern.
    /// </summary>
    public class NotificationBridgeConnection : AUDPSocketConnection
    {

        #region constructors
        public NotificationBridgeConnection()
        { }
        #endregion

        /// <summary>
        /// The parent NotificationBridge
        /// </summary>
        private NotificationBridge NotificationBridge
        {
            get
            {
                return (NotificationBridge)CallerObject;
            }
        }       

        /// <summary>
        /// This method is invoked from NotificationBridge for each new incoming UDP 
        /// Since there will be a new instance for each incoming request 
        /// thread-safety is not necessary.
        /// </summary>
        /// <param name="state"></param>
        public override void DataReceived(Object state)
        {
            NotificationDispatcher NotificationDispatcher = NotificationBridge.Dispatcher;

            NameValueCollection header = new NameValueCollection(); 

            SerializationReader reader = new SerializationReader((Byte[])state);
            String line = "";
            do
            {
                try
                {
                    line = (String)reader.ReadObject();
                    if (line == Environment.NewLine)
                        break;

                    Int32 ColonPos = line.IndexOf(':');
                    //String[] split = line.Split(new char[] { ':' });
                    //if (split.Length != 2)
                    if (ColonPos < 0)
                        throw new NotificationException_InvalidMulticastHeader("Invalid Header entry: " + line);

                    //header.Add(split[0].ToLower(), split[1].Trim());
                    header.Add(line.Substring(0, ColonPos).ToLower(), line.Substring(ColonPos + 1).Trim());

                    // Is sender the same Dispatcher? Than the message is already handled!
                    if (header["dispatcher"] != null && header["dispatcher"].Contains(NotificationDispatcher.Uuid))
                        return;
                }
                catch (NotificationException_InvalidMulticastHeader imh)
                {
                    throw imh;
                }
                catch (Exception e)
                {
                    throw new NotificationException_InvalidMulticastHeader("Header must ends with newline!", e);
                }

            } while (line != Environment.NewLine);

            if (header["notificationtype"] == null)
                throw new NotificationException_InvalidMulticastHeader("Header must contain a NotificationType!");

            if (header["dispatcher"] == null)
                throw new NotificationException_InvalidMulticastHeader("Header must contain the Dispatcher who sent this message!");

            if (header["senderid"] == null)
                throw new NotificationException_InvalidMulticastHeader("Header must contain the SenderID!");

            // The Dispatcher should only recieve notifications from the restricted VFS (if RestrictedSenderID was defined)
            if (NotificationDispatcher.RestrictedSenderID != String.Empty && NotificationDispatcher.RestrictedSenderID != header["senderid"])
                return;

            ANotificationType notificationType = NotificationBridge.Dispatcher.GetEmptyNotificationTypeFromFullBaseName(header["notificationtype"]);
            NotificationMessage message = new NotificationMessage((Byte[])reader.ReadObject(), notificationType.GetEmptyArgumentInstance());
            message.SenderInfo = new SenderInfo();
            message.SenderInfo.Host = header["host"];
            message.SenderInfo.DispatcherGuid = header["dispatcher"];
            message.SenderInfo.SenderID = header["senderid"];

            NotificationDispatcher.SendNotification(message, false);

        }
    }
}
