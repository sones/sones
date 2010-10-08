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
