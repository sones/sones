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
