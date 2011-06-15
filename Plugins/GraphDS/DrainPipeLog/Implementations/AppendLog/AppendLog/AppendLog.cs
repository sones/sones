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
using sones.Library.NewFastSerializer;

namespace sones.Plugins.GraphDS.DrainPipeLog.Storage
{
    public class AppendLog
    {
        private FileStream DatabaseFile;
        private FileStream DatabaseIndexFile;
        private SerializationWriter _writer = new SerializationWriter();
        private SerializationReader _reader = new SerializationReader();
        private bool FlushOnWrite = false;
        // todo: performance testing and eventual write every n seconds...

        /// <summary>
        /// a simple AppendLog which write data chunks of different sizes to the disk
        /// </summary>
        /// <param name="DatabaseFilename">the base file name (and location) of the database on disk</param>
        /// <param name="createNew">shall we create new files, regardless of their previous existence?</param>
        /// <param name="_FlushOnWrite">shall we flush on every write?</param>
        public AppendLog(String DatabaseFilename, bool createNew, bool _FlushOnWrite = true)
        {
            FlushOnWrite = _FlushOnWrite;
            if (createNew)
            {
                DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.CreateNew);
                DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.CreateNew);
            }
            else
            {
                if (File.Exists(DatabaseFilename + ".data"))
                {
                    // open the file
                    DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.Open);
                    DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.Open);
                }
                else
                {
                    DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.CreateNew);
                    DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.CreateNew);
                }
            }
        }

        public void Shutdown()
        {
            DatabaseFile.Flush();
            DatabaseIndexFile.Flush();
            DatabaseFile.Close();
            DatabaseIndexFile.Close();
        }

        /// <summary>
        /// writes a new chunk of data to the disk (and index)
        /// </summary>
        /// <param name="Data">the byte array with the data</param>
        public void Write(byte[] Data)
        {
            // write the data to the database file
            OnDiscAdress adress = WriteToDatabase(Data);
            // and the adress to the index
            WriteToIndex(adress);
        }

        /// <summary>
        /// reads an OnDiscAdress from the index. 
        /// </summary>
        /// <param name="NumberOfAdress">is the number of the OnDiscAdress to be read</param>
        /// <returns></returns>
        public OnDiscAdress ReadOnDiscAdress(long NumberOfAdress)
        {
            byte[] Readin;
            lock (DatabaseIndexFile)
            {
                // check if this is even possible...
                if (DatabaseIndexFile.Length <= NumberOfAdress * 33)
                    return null;

                // seek to the multiple of NumberOfAdress
                DatabaseIndexFile.Seek(NumberOfAdress * 33, SeekOrigin.Begin);

                byte[] _SerializedData;
                OnDiscAdress _deserializedAdress;

                try
                {
                    _SerializedData = new byte[33];
                    // todo: maybe a read cache might be great, to read more bytes sequential
                    DatabaseIndexFile.Read(_SerializedData, 0, 33);

                    _deserializedAdress = new OnDiscAdress();
                    _reader.Data = _SerializedData;
                    _deserializedAdress.Deserialize(ref _reader);

                    return _deserializedAdress;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public byte[] ReadData(OnDiscAdress Adress)
        {
            byte[] Readin;

            lock (DatabaseFile)
            {
                // seek to the position
                DatabaseFile.Seek(Adress.Start, SeekOrigin.Begin);
                Readin = new byte[Adress.End - Adress.Start];
                // read it in
                DatabaseFile.Read(Readin, 0, Readin.Length);
            }
            return Readin;
        }

        private void WriteToIndex(OnDiscAdress Adresspattern)
        {
            lock (DatabaseIndexFile)
            {
                // seek to the end...
                DatabaseIndexFile.Seek(DatabaseIndexFile.Length, SeekOrigin.Begin);
                // re-use the writer
                _writer.ResetBuffer();
                // serialize the index...
                byte[] ToWrite = Adresspattern.SerializeAligned(ref _writer);

                DatabaseIndexFile.Write(ToWrite, 0, ToWrite.Length);
                if (FlushOnWrite)
                    DatabaseIndexFile.Flush();
            }
        }

        private OnDiscAdress WriteToDatabase(byte[] ToWrite)
        {
            OnDiscAdress OutputAdress = new OnDiscAdress();

            lock (DatabaseFile)
            {
                // seek to the end...
                DatabaseFile.Seek(DatabaseFile.Length, SeekOrigin.Begin);
                OutputAdress.Start = DatabaseFile.Position;
                DatabaseFile.Write(ToWrite, 0, ToWrite.Length);
                OutputAdress.End = DatabaseFile.Position;

                if (FlushOnWrite)
                    DatabaseFile.Flush();
            }
            return OutputAdress;
        }
    }
}
