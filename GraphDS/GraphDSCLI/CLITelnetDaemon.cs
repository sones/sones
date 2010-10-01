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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using sones.Lib.Networking.TCPSocket;
//using System.Net.Sockets;
//using System.IO;
//using sones.Lib.DataStructures;
//using sones.Lib.Networking.Telnet;

//namespace sones.Lib.GraphCLI
//{
//    public class CLITelnetDaemon
//    {

//        private TCPSocketListener<TelnetServer<GraphCLI>> CLITelnetListener;

//        public CLITelnetDaemon(UInt16 myTelnetPort, object myGraphVFS, String myPath, params Type[] myCommandTypes)
//        {
//            Tuple<Object, String, Type[]> tupel = new Tuple<object, string, Type[]>
//                (myGraphVFS, myPath, myCommandTypes);

//            CLITelnetListener = new TCPSocketListener<TelnetServer<GraphCLI>>(myTelnetPort);

//            CLITelnetListener.SetDataObject(tupel);
//            CLITelnetListener.ClientTimeout = 60 * 1000;
//            CLITelnetListener.OnExceptionOccured += new TCPSocketExceptionOccuredHandler(CLITelnetListener_OnExceptionOccured);
//            CLITelnetListener.StartListener();
//        }

//        void CLITelnetListener_OnExceptionOccured(object mySender, Exception myException)
//        {
//            System.Diagnostics.Debug.WriteLine("[CLITelnetDaemon] CLITelnetListener_OnExceptionOccured: " + mySender + " Exception: " + myException);
//        }

//        public void SetTimeout(Int32 myTimeout)
//        {
//            CLITelnetListener.ClientTimeout = myTimeout;
//        }

//        public void Shutdown()
//        {
//            CLITelnetListener.StopAndWait();
//        }
//    }
//}
