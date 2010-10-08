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
using sones.Lib.ErrorHandling;
using sones.GraphFS.Objects;

#endregion

namespace sones.GraphFS
{

    public interface IGraphFSStream : IFSObjectOntology, IObjectLocation
    {

        GraphFSStreamUUID StreamUUID { get; }

        Boolean CanRead  { get; }
        Boolean CanSeek  { get; }
        Boolean CanWrite { get; }
        Boolean IsClosed { get; }
        Boolean IsAsync  { get; }

        UInt64  Position { get; }
        UInt64  Length   { get; }

        Exceptional Open(FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize);

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

