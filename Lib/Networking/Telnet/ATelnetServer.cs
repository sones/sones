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

/* ATelnetServer
 * (c) Stefan Licht, 2009
 * 
 * Your own telnet server must derive this ATelnetServer abstract class. The 
 * ConnectionEstablished method is invoked for a new connection and could be 
 * used for same initializing data
 * 
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace sones.Networking.Telnet
{
    public abstract class ATelnetServer
    {
        public abstract void ConnectionTimeout();

        public virtual void ConnectionEstablished() { }

        public TelnetParser TelnetConnection;

        public Boolean StopRequested;

        public Int32 Timeout;

        public Object DataObject;

        public void TelnetClose()
        {
            CloseTelnetConnectionDelegate();
        }

        public void TelnetWrite(Byte[] myBuffer, Int32 myOffset, Int32 mySize)
        {
            WriteDelegate(myBuffer, myOffset, mySize);
        }
        
        public void TelnetWriteByte(Byte myByte)
        {
            WriteByteDelegate(myByte);
        }

        internal Func<Boolean> CloseTelnetConnectionDelegate;
        internal Func<Byte[], Int32, Int32, Boolean> WriteDelegate;
        internal Func<Byte, Boolean> WriteByteDelegate;


    }
}
