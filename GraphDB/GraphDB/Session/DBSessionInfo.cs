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
 * DBSessionInfo
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB.Session
{

    public class DBSessionInfo : ISessionInfo
    {

        #region Properties

        #region SessionUUID

        /// <summary>
        /// The current SessionUUID
        /// </summary>
        public SessionUUID SessionUUID  { get; private set; }

        #endregion

        #region Username

        /// <summary>
        /// The current user
        /// </summary>
        public String Username          { get; private set; }

        #endregion

        #region ThrowExceptions

        /// <summary>
        /// Methods must or must not throw exceptions
        /// </summary>
        public Boolean ThrowExceptions { get; set; }

        #endregion        

        #endregion

        #region Constructors

        public DBSessionInfo(String myUsername)
        {
            Username        = myUsername;
            SessionUUID     = SessionUUID.NewUUID;
            ThrowExceptions = true;
        }

        #endregion

    }

}
