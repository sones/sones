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
using System.IO;
using System.Runtime.Remoting;

namespace sones.Library.BinaryStreamStructure
{       
    /// <summary>
    /// This class realise an proxy object for streams
    /// </summary>
    public sealed class StreamProxy : Stream
    {
        #region data

        /// <summary>
        /// The internal stream.
        /// </summary>
        private Stream _Stream;

        /// <summary>
        /// The internal stream position.
        /// </summary>
        private Int64 _Position;

        #endregion

        #region constructor

        /// <summary>
        /// The constructor for the proxy object.
        /// </summary>
        /// <param name="myStream">The external stream for the proxy.</param>
        public StreamProxy(Stream myStream)
        {
            _Stream = myStream;
            _Position = 0;
        }        

        #endregion

        #region abstract class stream

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, Object state)
        {
            lock (_Stream)
            {
                _Stream.Position = _Position;
                return _Stream.BeginRead(buffer, offset, count, callback, state);
            }            
        }

        public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, Object stateObject)
        {
            return null;
        }

        public override void Close()
        {
            
        }

        public void CopyTo(Stream destination)
        {
            _Stream.CopyTo(destination);
        }

        public void CopyTo(Stream destination, int bufferSize)
        {
            _Stream.CopyTo(destination, bufferSize);
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return _Stream.CreateObjRef(requestedType);
        }

        public void Dispose()
        {
            
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            lock (_Stream)
            {
                return _Stream.EndRead(asyncResult);
            }
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            
        }

        public override bool Equals(Object obj)
        {
            if ((obj as StreamProxy) == null)
            {
                return false;
            }
            else
            {
                var stream = (StreamProxy)obj;

                return _Stream.Equals(stream._Stream);
            }            
        }

        public override int GetHashCode()
        {
            return _Stream.GetHashCode();
        }

        public Object GetLifetimeService()
        {
            return _Stream.GetLifetimeService();
        }

        public Type GetType()
        {
            return _Stream.GetType();
        }

        public override Object InitializeLifetimeService()
        {
            return _Stream.InitializeLifetimeService();
        }

        public override string ToString()
        {
            return _Stream.ToString();
        }

        public override int ReadByte()
        {
            lock (_Stream)
            {
                _Stream.Position = _Position;
                
                var value = _Stream.ReadByte();
                _Position += value;

                return value;
            }            
        }

        public override bool CanRead
        {
            get { return _Stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _Stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            
        }

        public override long Length
        {
            get { return _Stream.Length; }
        }

        public override long Position
        {
            get
            {
                return _Position;
            }
            set
            {                
                _Position = value;                
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_Stream)
            {
                _Stream.Position = _Position;

                var value = _Stream.Read(buffer, offset, count);
                _Position += value;

                return value;
            }            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_Stream)
            {
                _Stream.Seek(_Position, origin);
                _Position = _Stream.Seek(offset, origin);
            }
            
            return _Position;
        }

        public override void SetLength(long value)
        {
            
        }        

        public override void Write(byte[] buffer, int offset, int count)
        {
            
        }

        public override void WriteByte(byte value)
        {
            
        }

        #endregion       
    }
}
