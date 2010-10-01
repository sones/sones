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

/* GraphLib - NDatabaseMisc
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Notifies about an Misc. Database Event
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using sones.Notifications;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Notification.NotificationTypes.Database
{
    /// <summary>
    /// Notifies about an Misc. Database Event
    /// </summary>
    public class NDatabaseMisc : NotificationGroupDatabase
    {

        public new class Arguments : INotificationArguments
        {
            public String Message;

            #region Constructors

            public Arguments() { }

            public Arguments(String MyMessage)
            {
                Message = MyMessage;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(Message);
               
                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                Message = _SerializationReader.ReadString();
            }

            #endregion
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about Miscelleanous Database Events"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
