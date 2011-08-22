/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.GraphDS.UDC
{
    public class UDC_Client_Thread
    {
        private Boolean Stop = false;
        public Boolean ShutdownComplete = false;
        private Int32 WaitUpFront = 1 * 60 * 1000;  // 1 minute
        private Int32 UpdateInterval = 30 * 60 * 1000; // 30 minutes

        #region Constructor
        public UDC_Client_Thread(Int32 _WaitUpFront, Int32 _UpdateInterval)
        {
            if (_WaitUpFront != 0)
                WaitUpFront = _WaitUpFront;
            if (_UpdateInterval != 0)
                UpdateInterval = _UpdateInterval;
        }
        #endregion

        public void Shutdown()
        {
            Stop = true;
        }

        #region Main Loop - Polling Mode
        public void Run()
        {
            while (!Stop)
            {
                Thread.Sleep(UpdateInterval); // wait the time which was pre-configured
            }

            ShutdownComplete = true;
        }
        #endregion
    }
}
