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


/* PandoraLib - NServiceDiscovery_Announcement
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Announces a Service.
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings

using System;
using System.Net;
using System.Text;

using sones.Notifications;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Notifications.Exceptions;

#endregion

namespace sones.Notifications.Autodiscovery.NotificationTypes
{

    /// <summary>
    /// Announces a Service.
    /// </summary>
    public class NSD_Filesystem : NServiceDiscovery_Filesystem
    {
        public new class Arguments : INotificationArguments
        {

            #region Data
            public String ServiceGlobalUniqueName;                      // the name of this service - it determines the uniqueness of this service
            public Uri ServiceUri;
            public DiscoverableServiceType ServiceType;    // the Type of this Service
            #endregion

            #region Constructors

            public Arguments()
            {
            }

            public Arguments(String _ServiceName, Uri _ServiceUri, DiscoverableServiceType _ServiceType)
            {
                ServiceGlobalUniqueName = _ServiceName;
                ServiceUri = _ServiceUri;
                ServiceType = _ServiceType;
            }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                SerializationWriter writer = new SerializationWriter();

                writer.WriteObject(ServiceGlobalUniqueName);
                writer.WriteObject(ServiceUri.ToString());
                writer.WriteObject(ServiceType);
               
                return writer.ToArray();

            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                SerializationReader reader = new SerializationReader(mySerializedBytes);

                ServiceGlobalUniqueName = (String)reader.ReadObject();
                String _ServiceUri   = (String)reader.ReadObject();
                ServiceType = (DiscoverableServiceType)reader.ReadObject();

                if (!Uri.TryCreate(_ServiceUri,UriKind.Absolute,out ServiceUri))
                    throw new NotificationException_InvalidNotificationPayload("IP not parseable. Notification Packet invalid!");
            }

            #endregion

        }

        #region ANotificationType

        public override String Description
        {
            get { return "Announces a service."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }

}
