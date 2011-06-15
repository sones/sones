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
using sones.Plugins.GraphDS.DrainPipeLog.Storage;
using System.Threading;

namespace sones.Plugins.GraphDS.DrainPipeLog
{
    public class WriteThread
    {
        // this is the buffer which holds the data
        private List<byte[]> Async_Buffer = null;
        public Int64 BytesInAsyncBuffer = 0;
        private AppendLog internalAppendLog = null;
        private Boolean Stop = false;
        public Boolean ShutdownComplete = false;

        #region Constructor
        public WriteThread(AppendLog _AppendLog)
        {
            internalAppendLog = _AppendLog;
            Async_Buffer = new List<byte[]>();
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
                lock (Async_Buffer)
                {
                    if (Async_Buffer.Count > 0)
                    {
                        data = Async_Buffer[0];
                        Async_Buffer.RemoveAt(0);
                        BytesInAsyncBuffer = BytesInAsyncBuffer - data.Length;
                    }
                    
                }

                // finally write it...
                if (data != null)
                { 
                    internalAppendLog.Write(data); // write it
                    data = null; // null it
                }
                else
                    Thread.Sleep(1); // wait at least 1 msec if nothing to do
            }
            // handle shutdown event by writing everything in the buffer to disk...
            while (Async_Buffer.Count != 0)
            {
                data = Async_Buffer[0];
                Async_Buffer.RemoveAt(0);
                BytesInAsyncBuffer = BytesInAsyncBuffer - data.Length;
                internalAppendLog.Write(data); // write it
            }
            data = null;

            ShutdownComplete = true;
        }
        #endregion

        /// <summary>
        /// adds a chunk of data to the buffer, regardless how big the buffer already is, checking
        /// of the maximum size needs to be done one layer above
        /// </summary>
        /// <param name="WriteData">the byte array to be written</param>
        public void Write(byte[] WriteData)
        {
            lock(Async_Buffer)
            {
                Async_Buffer.Add(WriteData);
                BytesInAsyncBuffer = BytesInAsyncBuffer + WriteData.Length;
            }
        }

    }
}
