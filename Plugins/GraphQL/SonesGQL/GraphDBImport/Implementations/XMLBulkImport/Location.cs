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
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    internal class Location: IEquatable<Location>
    {
        public Location(String myLocation)
        {
            LocationPath = myLocation;
            InUse = false;
        }

        public string LocationPath { get; private set; }

        public bool InUse { get; private set; }

        private CancellationTokenSource _cancel;

        private object _lock = new object();
        public string CurrentFile { get; private set; }

        public void Cancel()
        {
            if (_cancel != null)
                _cancel.Cancel();
        }


        public void Store(IEnumerable<IncomingEdge> myEdges, String myFile)
        {
            SetUsed();

            Console.WriteLine("Started: {0}", myFile);
            CurrentFile = myFile;

            _cancel = new CancellationTokenSource();

            ExecuteStore(myFile, myEdges);

            Console.WriteLine("Finished: {0}", myFile);
            SetUnused();
        }

        private void SetUnused()
        {
            lock (_lock)
            {
                InUse = false;
            }
        }

        private void SetUsed()
        {
            lock (_lock)
            {
                if (InUse)
                    throw new InvalidOperationException("Location is currently in use.");

                InUse = true;
            }
        }

        private void ExecuteStore(String myPath, IEnumerable<IncomingEdge> myEdges)
        {
            var stream = new FileStream(myPath, FileMode.Create, FileAccess.Write, FileShare.None, Environment.SystemPageSize, FileOptions.SequentialScan);
            try
            {
                BinaryWriter writer = new BinaryWriter(stream);
                foreach (var edge in myEdges)
                {
                    if (_cancel.Token.IsCancellationRequested)
                        break;

                    WriteIncomingEdge(writer, edge);
                }
            }
            finally
            {
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
        /// Stores one incoming edge to the file.
        /// </summary>
        /// <param name="writer">A writer, that accesses the file.</param>
        /// <param name="myEdge">The edge to be stored.</param>
        private static void WriteIncomingEdge(BinaryWriter writer, IncomingEdge myEdge)
        {
            writer.Write(myEdge.TargetVertexTypeID);
            writer.Write(myEdge.TargetVertexID);

            writer.Write(myEdge.SourceVertexTypeID);
            writer.Write(myEdge.SourceVertexID);

            writer.Write(myEdge.PropertyID);

        }

        #region IEquatable<Location> Members

        public bool Equals(Location other)
        {
            return other != null && LocationPath.Equals(other.LocationPath);
        }

        #endregion

        #region Object Members

        public override bool Equals(object obj)
        {
            return Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return LocationPath.GetHashCode();
        }

        public override string ToString()
        {
            return LocationPath.ToString();
        }

        #endregion
    }
}
