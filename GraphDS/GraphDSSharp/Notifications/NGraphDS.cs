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

/*
 * NGraphDS
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Net;
using System.Text;

using sones.Notifications;
using sones.Notifications.NotificationTypes;

#endregion


namespace sones.GraphDS.API.CSharp.Notifications
{

    /// <summary>
    /// Groups all notifications of a GraphDS instance
    /// </summary>

    public class NGraphDS : ANotificationType
    {

        public NGraphDS()
            : base(IPAddress.Parse("224.10.10.30"), 5000)
        { }

        public override string Description
        {
            get { return "Groups all notifications of a GraphDS instance."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }

    }

}
