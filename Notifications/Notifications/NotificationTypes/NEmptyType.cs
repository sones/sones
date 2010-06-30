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

namespace sones.Notifications.NotificationTypes
{
    public class NEmptyType : ANotificationType
    {

        public NEmptyType()
            : base(System.Net.IPAddress.Parse("224.10.10.1"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all emtpy test Notifications."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
