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

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    /// <summary>
    /// Converts a file into an enumeration of incoming edges.
    /// </summary>
    /// <remarks>
    /// An instance of this class reads all incoming edges from file.
    /// This happens in a buffered way, so that not all incoming edges of the file have to be stored in main memory.
    /// Each instance can only be used one time. 
    /// </remarks>
    internal class ChunkReader: IEnumerable<IncomingEdge>, IDisposable
    {
        #region Data

        /// <summary>
        /// Stores the file stream.
        /// </summary>
        private FileStream _stream;

        /// <summary>
        /// Stores the reader on the file stream.
        /// </summary>
        private BinaryReader _reader;

        #endregion

        #region public
        /// <summary>
        /// Creates a new instance of class ChunkReader.
        /// </summary>
        /// <param name="myFilename">The path to a file, that stores incoming edges.</param>
        /// <param name="myRemoveFileAfterRead">If true, the file is removed from disk, when this instance is disposed or reaches the end of the file, otherwise not.</param>
        public ChunkReader(String myFilename, bool myRemoveFileAfterRead = false)
        {
            #region Open the file
            //A hint to the file system.
            var options = FileOptions.SequentialScan;

            //file will be deleted, after closure of the stream.
            if (myRemoveFileAfterRead)
                options |= FileOptions.DeleteOnClose;

            //Opens the file.
            _stream = new FileStream(myFilename, FileMode.Open, FileAccess.Read, FileShare.None, Environment.SystemPageSize, FileOptions.SequentialScan);
            _reader = new BinaryReader(_stream);

            #endregion

            #region fill properties

            RemoveAfterRead = myRemoveFileAfterRead;
            Filename = myFilename;

            #endregion
        }

        /// <summary>
        /// Gets whether the file will be removed when this instance is disposed or it reaches the end of the file.
        /// </summary>
        public bool RemoveAfterRead { get; private set; }

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Filename { get; private set; }
        
        #endregion

        #region private

        /// <summary>
        /// Reads the file and converts it into an enumeration of incoming edges.
        /// </summary>
        /// <remarks>
        /// The data is read out as recently as it is accessed.
        /// </remarks>
        /// <returns>Returns an enumeration of incoming edges.</returns>
        private IEnumerable<IncomingEdge> Read()
        {
            lock (_stream)
            {
                if (_stream.Position != 0L)
                    throw new InvalidOperationException("The enumerator can only be accessed one time.");

                while (_stream.Position + IncomingEdge.Size <= _stream.Length)
                    yield return ReadIncomingEdge();

                _reader.Close();
            }
        }

        /// <summary>
        /// Reads the next incoming edge from file.
        /// </summary>
        /// <remarks>
        /// This method reads the next incoming edge from file and moves the file pointer back.
        /// </remarks>
        /// <returns>Returs an incoming edge.</returns>
        private IncomingEdge ReadIncomingEdge()
        {
            var TargetVertexTypeID = _reader.ReadInt64();
            var TargetVertexID = _reader.ReadInt64();

            var SourceVertexTypeID = _reader.ReadInt64();
            var SourceVertexID = _reader.ReadInt64();

            var PropertyID = _reader.ReadInt64();

            return new IncomingEdge
            {
                PropertyID = PropertyID,
                SourceVertexID = SourceVertexID,
                SourceVertexTypeID = SourceVertexTypeID,
                TargetVertexID = TargetVertexID,
                TargetVertexTypeID = TargetVertexTypeID
            };
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<IncomingEdge> GetEnumerator()
        {
            return Read().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Read().GetEnumerator();
        }

        #endregion

        #region IDisposable Members

		public void Dispose()
        {
            _reader.Close();
        }
 
	    #endregion    
    }
}
