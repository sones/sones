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


/* PandoraLib - NStorageEngine_WriteQueueEmpty
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
using sones.Notifications.NotificationTypes;

namespace sones.StorageEngines.Notification
{

    /// <summary>
    /// Notifies about an WriteQueue which is empty.
    /// </summary>

    public class NStorageEngine_WriteQueueEmpty : NStorageEngine
    {

        public new class Arguments : INotificationArguments
        {
            public String StorageLocation;

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                return new Byte[0];
            }

            public void Deserialize(byte[] mySerializedBytes)
            { }

            #endregion
        }

        #region INotificationType Members

        public override string Description
        {
            get { return "Notifies about an WriteQueue which is empty"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
