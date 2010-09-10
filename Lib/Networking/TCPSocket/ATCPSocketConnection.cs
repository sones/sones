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

/* PandoraWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This abstract class is the TCP connection client abstraction.
 * If you derive from this class and use the derived class as generic-type 
 * parameter for a TCPSocketListener instance and implement the ConnectionEstablished()
 * method, this method will be invoked for each webrequest on this port.
 * If KeepAlive is false, each request will handled by a new instance of your class! Otherwise,
 * use WaitForStreamDataAvailable() to wait for new data...
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using sones.Networking.HTTP;
using System.Diagnostics;
using System.Threading;

namespace sones.Networking.TCPSocket
{
    public abstract class ATCPSocketConnection : IDisposable
    {

        public event ExceptionOccuredHandler OnExceptionOccured;
                
        /// <summary>
        /// The TcpClient connection to a connected Client
        /// </summary>
        public TcpClient TcpClientConnection {get; set;}

        /// <summary>
        /// Is False if the client is disconnected from the server
        /// </summary>
        public Boolean IsConnected { get; set; }

        /// <summary>
        /// Will be invoked when a client established a connection to the server. Put you logic in this method.
        /// </summary>
        /// <returns>True, if the connection was handled succesfully. False will end the connection, regardless the KeepAlive setting!</returns>
        public abstract bool ConnectionEstablished();

        /// <summary>
        /// This Object will be routed to each connected client ConnectionEstablished
        /// </summary>
        public Object DataObject { get; set; }

        /// <summary>
        /// The Client ConnectionEstablished should timeout after this Timeout in Milliseconds - should be impemented in ConnectionEstablished logic
        /// </summary>
        public Int32 Timeout { get; set; }

        /// <summary>
        ///  The connection is keepalive
        /// </summary>
        public Boolean KeepAlive { get; set; }

        /// <summary>
        /// Server requested stopping
        /// </summary>
        public Boolean StopRequested { get; set; }

        /// <summary>
        /// Wait until new StreamData is available timeout or server shutdown
        /// </summary>
        /// <returns>True: if new StreamData is available. False: if timeout or server shutdown</returns>
        public Boolean WaitForStreamDataAvailable()
        {

            #region Timeout

            var Start = DateTime.Now;

            if (TcpClientConnection == null)
                return false;

            var stream = TcpClientConnection.GetStream();

            if (stream == null)
                return false;

            while (!StopRequested && TcpClientConnection.Connected
                    && !stream.DataAvailable
                    && ((Timeout == System.Threading.Timeout.Infinite) || (DateTime.Now.Subtract(Start).TotalMilliseconds < Timeout)))
            {
                Thread.Sleep(1);
            }

            #endregion

            if (StopRequested || !TcpClientConnection.Connected)
            {
                Debug.WriteLine("[ATcpSocketConnection][StreamDataAvailableTimeout] Stop requested");
                return false;
            }

            // If we have any DataAvailable than proceed, even if StopRequested is true
            if (stream.DataAvailable)
                return true;

            if (DateTime.Now.Subtract(Start).TotalMilliseconds >= Timeout)
                Debug.WriteLine("[ATcpSocketConnection][StreamDataAvailableTimeout] timedout after " + Timeout + "ms");

            return false;

        }

        public Boolean GetHeaderAndBody(NetworkStream myStream, out HTTPHeader myHttpHeader, out Byte[] myBody)
        {

            myHttpHeader = null;
            myBody = null;
            
            #region Data Definition

            var ReadBuffer = new Byte[16 * 1024];

            Int32 BytesRead = 0;
            var FirstBytesList = new List<Byte>();

            #endregion

            #region Read the FirstBytes until no Data is available or we read more than we can store in a List of Bytes

            do
            {
                
                BytesRead = myStream.Read(ReadBuffer, 0, ReadBuffer.Length);
                
                if (BytesRead == ReadBuffer.Length)
                {
                    FirstBytesList.AddRange(ReadBuffer);
                }

                else
                {
                    var _TempBytes = new Byte[BytesRead];
                    Array.Copy(ReadBuffer, 0, _TempBytes, 0, BytesRead);
                    FirstBytesList.AddRange(_TempBytes);
                }

            }
            while (myStream.DataAvailable && BytesRead > 0 && FirstBytesList.Count < (Int32.MaxValue - ReadBuffer.Length));

            #endregion

            #region Find Header

            Int32 CurPos = 4;
            Byte[] FirstBytes = FirstBytesList.ToArray();

            if (FirstBytes.Length <= CurPos)
            {

                // If header length < 4 we have an invalid header
                myStream.Close();

                return false;

            }

            while ((CurPos < FirstBytes.Length) && !(FirstBytes[CurPos - 4] == 13 && FirstBytes[CurPos - 3] == 10 && FirstBytesList[CurPos - 2] == 13 && FirstBytesList[CurPos - 1] == 10))
            {
                CurPos++;
            }

            var HeaderBytes = new Byte[CurPos];
            Array.Copy(FirstBytes, 0, HeaderBytes, 0, CurPos);

            var HeaderString = Encoding.UTF8.GetString(HeaderBytes);
            myHttpHeader = new HTTPHeader(HeaderString);

            #endregion

            #region Body

            myBody = new Byte[myHttpHeader.ContentLength];
            
            if (myHttpHeader.ContentLength > 0)
            {

                if (myHttpHeader.ContentLength < (UInt64)(FirstBytes.Length - CurPos))
                {
                    // contentlength defined in the header is lower than the recieved bytes
                    return false;
                }

                Array.Copy(FirstBytes, CurPos, myBody, 0, FirstBytes.Length - CurPos);

                // Read the rest of the bytes
                if (myHttpHeader.ContentLength > (UInt64)(FirstBytes.Length - CurPos))
                {
                    
                    var TotalBytesRead = (Int64) FirstBytes.Length - CurPos;

                    while ((UInt64) TotalBytesRead < myHttpHeader.ContentLength)
                    {
                        if (!WaitForStreamDataAvailable())
                            return true;

                        BytesRead = myStream.Read(ReadBuffer, 0, ReadBuffer.Length);
                        Array.Copy(ReadBuffer, 0, myBody, TotalBytesRead, BytesRead);
                        TotalBytesRead += (Int64)BytesRead;
                    }

                }

            }

            #endregion

            return true;

        }

        /// <summary>
        /// This method is called, directly after the Socket recieved the first client-connection. 
        /// If you do not have KeepAlive connections, this method will be invoked for each new connection.
        /// For KeepAlive connection, this method will be invoked only one time.
        /// </summary>
        public virtual void InitializeSocket(object myDataObject)
        {
        }

        /// <summary>
        /// Read all CURRENTLY available data (in the kernel) from the Stream. Keep in mind, that there might be more data than this method returns.
        /// </summary>
        /// <returns>A bunch of data which arrived at the kernel.</returns>
        public Byte[] GetAllAvailableData()
        {

            #region Data Definition

            NetworkStream networkStream = TcpClientConnection.GetStream();

            Byte[] ReadBuffer = new Byte[1024];

            Int32 BytesRead = 0;
            List<Byte> FirstBytesList = new List<Byte>();

            #endregion

            #region Read the FirstBytes until no Data is available or we read more than we can store in a List of Bytes

            do
            {
                BytesRead = networkStream.Read(ReadBuffer, 0, ReadBuffer.Length);
                if (BytesRead == ReadBuffer.Length)
                {
                    FirstBytesList.AddRange(ReadBuffer);
                }
                else
                {
                    Byte[] Temp = new Byte[BytesRead];
                    Array.Copy(ReadBuffer, 0, Temp, 0, BytesRead);
                    FirstBytesList.AddRange(Temp);
                }
            } while (networkStream.DataAvailable && BytesRead > 0 && FirstBytesList.Count < (Int32.MaxValue - ReadBuffer.Length));

            #endregion

            return FirstBytesList.ToArray();

        }

        public void ExceptionThrowed(Object mySender, Exception myException)
        {
            if (OnExceptionOccured != null)
            {
                OnExceptionOccured(this, myException);
            }
        }


        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
