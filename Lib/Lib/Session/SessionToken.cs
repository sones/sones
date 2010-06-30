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
 * GraphFS - SessionToken
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.Settings;

#endregion

namespace sones.Lib.Session
{

    /// <summary>
    /// TODO: remove inheritance of SessionInfos! And remove the SessionInfos
    /// </summary>
    public class SessionToken
    {

        private ISessionInfo _SessionInfo;
        public ISessionInfo SessionInfo
        {
            get { return _SessionInfo; }
            set { _SessionInfo = value; }
        }

        private SessionSettings _SessionSettings;
        public SessionSettings SessionSettings
        {
            get { return _SessionSettings; }
            set { _SessionSettings = value; }
        }

        public SessionToken(ISessionInfo mySessionInfo)
        {
            _SessionInfo = mySessionInfo;
            //_Transactions = new LinkedList<Transaction>();
            _SessionSettings = new SessionSettings();
        }

    }
}
