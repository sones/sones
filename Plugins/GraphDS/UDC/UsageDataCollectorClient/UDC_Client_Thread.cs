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

        #region Constructor
        public UDC_Client_Thread()
        {
        }
        #endregion

        public void Shutdown()
        {
            Stop = true;
        }

        #region Main Loop - Polling Mode
        public void Run()
        {
            byte[] data = null;
            while (!Stop)
            {
                Thread.Sleep(1); // wait at least 1 msec if nothing to do
            }

            ShutdownComplete = true;
        }
        #endregion
    }
}
