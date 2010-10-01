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

/* <id name="Networking – HttpWebContext" />
 * <copyright file="HttpWebContext.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This context will be stored into the current thread</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Contexts;
using System.IO;

namespace sones.Networking.HTTP
{

    /// <summary>
    /// This context will be stored into the current thread
    /// </summary>
    public class HTTPContext
    {

        public const String ContextName = "sones.Network.HTTPContext";

        #region Constructors

        public HTTPContext(HTTPHeader myRequestHeader, Byte[] myRequestBody, HTTPHeader myResponseHeader, Stream myResponseStream)
        {
            _RequestHeader  = myRequestHeader;
            _RequestBody    = myRequestBody;
            _ResponseHeader = myResponseHeader;
            _ResponseStream = myResponseStream;
        }

        #endregion

        #region RequestHeader

        private HTTPHeader _RequestHeader;
        public HTTPHeader RequestHeader
        {
            get { return _RequestHeader; }
        }

        #endregion

        #region ResponseHeader

        private HTTPHeader _ResponseHeader;
        public HTTPHeader ResponseHeader
        {
            get { return _ResponseHeader; }
            set { _ResponseHeader = value; }
        }

        #endregion

        #region RequestBody

        private Byte[] _RequestBody;
        public Byte[] RequestBody
        {
            get { return _RequestBody; }
            set { _RequestBody = value; }
        }

        #endregion

        #region ResponseStream

        /// <summary>
        /// True if something was written into the response stream. The response header will NOT be send as response!
        /// </summary>
        public Boolean StreamDataAvailable
        {
            get { return _StreamDataAvailable; }
        }
        private Boolean _StreamDataAvailable;

        private Stream _ResponseStream;


        public void WriteToResponseStream(Byte[] myBuffer)
        {
            _ResponseStream.Write(myBuffer, 0, myBuffer.Length);
            _StreamDataAvailable = true;
        }

        /// <summary>
        /// Write some data into the response stream. The <see cref="ResponseHeader"/> will NOT be added to the response stream.
        /// </summary>
        /// <param name="myBuffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="myOffset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="myCount">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.ArgumentException">The sum of offset and count is greater than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset or count is negative</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public void WriteToResponseStream(Byte[] myBuffer, Int32 myOffset, Int32 myCount)
        {
            _ResponseStream.Write(myBuffer, myOffset, myCount);
            _StreamDataAvailable = true;
        }

        public void WriteToResponseStream(Stream myStream)
        {

            var _Buffer = new Byte[myStream.Length];
            myStream.Read(_Buffer, 0, (Int32) myStream.Length);

            _ResponseStream.Write(_Buffer, 0, (Int32) myStream.Length);
            _StreamDataAvailable = true;

        }

        public void WriteToResponseStream(Stream myStream, UInt64 myOffset, UInt64 myCount)
        {

            var _Buffer = new Byte[myStream.Length];
            myStream.Read(_Buffer, (Int32)myOffset, (Int32)myCount);

            _ResponseStream.Write(_Buffer, 0, (Int32)myStream.Length);
            _StreamDataAvailable = true;

        }

        #endregion

    }
}
