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

/*
 * GraphFS - IGraphFSStream
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings 

using System;
using System.IO;
using System.Text;

using System.Collections.Generic;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS
{

    public interface IGraphFSStream //: IPandoraObjectOntology
    {

        GraphFSStreamUUID StreamUUID { get; }

        Boolean CanRead  { get; }
        Boolean CanSeek  { get; }
        Boolean CanWrite { get; }
        Boolean IsClosed { get; }
        Boolean IsAsync  { get; }

        UInt64  Position { get; }
        UInt64  Length   { get; }

        #region IPandoraObjectOntology Members

        ObjectLocation ObjectLocation { get; set; }
        String ObjectName { get; }
        String ObjectPath { get; set; }

        String ObjectEdition { get; set; }
        IDictionary<String, ObjectEdition> ObjectEditions { get; }

        RevisionID ObjectRevisionID { get; set; }
        IDictionary<RevisionID, ObjectRevision> ObjectRevisions { get; }

        String ObjectStream { get; set; }
        IDictionary<String, ObjectStream> ObjectStreams { get; }

        UInt64 ObjectSize { get; }
        UInt64 ObjectSizeOnDisc { get; }

        #endregion


        // Do NOT change this to UInt64!!!
        Boolean Seek(SeekOrigin myOrigin);
        Boolean Seek(Int64 myStreamOffset, SeekOrigin myOrigin);

        void SetLength(UInt64 myNewLength);

        UInt64 Read(Byte[] myArray, UInt64 myArrayOffset, UInt64 myNumberOfBytesToRead);
        UInt64 Read(Byte[] myArray, UInt64 myArrayOffset, SeekOrigin myOrigin, UInt64 myNumberOfBytesToRead);
        UInt64 Read(Byte[] myArray, UInt64 myArrayOffset, Int64 myStreamOffset, SeekOrigin myOrigin, UInt64 myNumberOfBytesToRead);

        void Write(Byte[] myArray);
        void Write(Byte[] myArray, SeekOrigin myOrigin);
        void Write(Byte[] myArray, Int64 myStreamOffset, SeekOrigin myOrigin);

        void Write(Byte[] myArray, UInt64 myArrayOffset, UInt64 myNumberOfBytesToBeWritten);
        void Write(Byte[] myArray, UInt64 myArrayOffset, SeekOrigin myOrigin, UInt64 myNumberOfBytesToBeWritten);
        void Write(Byte[] myArray, UInt64 myArrayOffset, Int64 myStreamOffset, SeekOrigin myOrigin, UInt64 myNumberOfBytesToBeWritten);


        void Lock(UInt64 myStreamPosition, UInt64 myNumberOfBytes);
        void Unlock(UInt64 myStreamPosition, UInt64 myNumberOfBytes);

        void Flush();
        void Close();

        void Dispose();
        

    }

}

