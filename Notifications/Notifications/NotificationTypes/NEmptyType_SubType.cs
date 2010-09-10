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

/* GraphLib - NEmptyType
 * (c) Stefan Licht, 2009
 * 
 * This is an empty Notification without any arguments and name.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.NotificationTypes
{
    public class NEmptyType_SubType : NEmptyType
    {

        public NEmptyType_SubType() { }

        public new class Arguments : INotificationArguments
        {

            public Arguments() { }

            public Arguments(Byte[] mySerializedData)
            {
                Deserialize(mySerializedData);
            }

            #region INotificationArguments Members

            public byte[] Serialize()
            { 
                return new Byte[0];
            }

            public void Deserialize(byte[] mySerializedBytes)
            { }

            public new String ToString()
            {
                return String.Empty;
            }

            #endregion
        }

        #region ANotificationType Members

        public override string Description
        {
            get { return "Just an empty test notification"; }
        }

        //public override string Name
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }
    }
}
