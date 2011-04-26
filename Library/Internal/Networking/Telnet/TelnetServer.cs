/* TelnetServer<TInstance>
 * (c) Stefan Licht, 2009
 * 
 * Create a class which derive from ATelnetServer and start a TCPSocketListener with the 
 * generic class TelnetServer (with a generic class of your class which should implement the
 * telnet server (e.g. GraphCLI))
 * CLITelnetListener = new TCPSocketListener<TelnetServer<GraphCLI>>(myTelnetPort);
 * 
 * To set some constructor data for the telnet server use the DataObject with
 * In your telnet server implementation of method ConnectionEstablished() you can use 
 * this DataObject to set your own data
 * CLITelnetListener.SetDataObject(tupel);
 * 
 * To send data to the telnet client, use: TelnetWrite()
 * 
 * To close the connection Serverside: TelnetClose()
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
using sones.Networking.TCPSocket;

namespace sones.Networking.Telnet
{
    public class TelnetServer<TInstance> : ATCPSocketConnection where TInstance : ATelnetServer, new()
    {

        private TelnetParser _TelnetParser;
        private TInstance _TInstance;

        public TelnetServer()
        {
            _TelnetParser = new TelnetParser();
        }

        public TelnetServer(UInt16 Port) : this()
        {
        }

        public override void InitializeSocket(object myDataObject)
        {
            _TInstance = new TInstance();
            ((ATelnetServer)_TInstance).TelnetConnection = _TelnetParser;
            //((ATelnetServer)_TInstance).TcpClientConnection = TcpClientConnection;
            ((ATelnetServer)_TInstance).StopRequested = StopRequested;
            ((ATelnetServer)_TInstance).Timeout = Timeout;
            //((ATelnetServer)_TInstance).DataObject = DataObject;

            ((ATelnetServer)_TInstance).CloseTelnetConnectionDelegate = CloseConnection;
            ((ATelnetServer)_TInstance).WriteDelegate = Write;
            ((ATelnetServer)_TInstance).WriteByteDelegate = WriteByte;

            KeepAlive = true;

            ((ATelnetServer)_TInstance).ConnectionEstablished();
        }

        public override bool ConnectionEstablished()
        {

            #region Wait for data or timeout

            // Wait until new StreamData is available (returns true), timeout or server shutdown
            if (!WaitForStreamDataAvailable())
            {

                ((ATelnetServer)_TInstance).ConnectionTimeout();
                return false;

            }

            #endregion

            #region Pass the data to the TelnetParse which will invoke some events

            Byte[] totalBytes = GetAllAvailableData();

            if (!_TelnetParser.Parse(totalBytes))
                throw new Exception("Could not parse telnet request!");

            #endregion

            return true;

        }

        private Boolean Write(Byte[] myBuffer, Int32 myOffset, Int32 mySize)
        {
            TcpClientConnection.GetStream().Write(myBuffer, myOffset, mySize);
            
            return true;
        }

        private Boolean WriteByte(Byte myByte)
        {
            TcpClientConnection.GetStream().WriteByte(myByte);

            return true;
        }
        private Boolean CloseConnection()
        {
            StopRequested = true;
            TcpClientConnection.GetStream().Close();

            return true;
        }


        public override void Dispose()
        {
            _TelnetParser = null;
            if (_TInstance is IDisposable)
            {
                ((IDisposable)_TInstance).Dispose();
            }
        }
    }
}
