using System;

namespace BplusDotNet
{
	/// <summary>
	/// Provides an indexed object which maps to buffers in an underlying file object
	/// </summary>
	public class BufferFile
	{
		System.IO.Stream fromfile;
		int headersize;
		// this should really be a read only property
		public int buffersize;
		long seekStart = 0;
		//byte[] header;
		public static byte[] HEADERPREFIX = { 98, 112, 78, 98, 102 };
		public const byte VERSION = 0;
		public const int INTSTORAGE = 4;
		public const int LONGSTORAGE = 8;
		public const int SHORTSTORAGE = 2;
		public static int MINBUFFERSIZE = 16;
		public BufferFile(System.IO.Stream fromfile, int buffersize, long seekStart)
		{
			this.seekStart = seekStart;
			this.fromfile = fromfile;
			this.buffersize = buffersize;
			this.headersize = HEADERPREFIX.Length + INTSTORAGE + 1; // +version byte+4 bytes for buffersize
			this.sanityCheck();
		}
		public BufferFile(System.IO.Stream fromfile, int buffersize):
			this(fromfile, buffersize, (long)0) 
		{
			// just start seek at 0
		}
		public static BufferFile SetupFromExistingStream(System.IO.Stream fromfile) 
		{
			return SetupFromExistingStream(fromfile, (long)0);
		}
		public static BufferFile SetupFromExistingStream(System.IO.Stream fromfile, long StartSeek) 
		{
			BufferFile result = new BufferFile(fromfile, 100, StartSeek); // dummy buffer size for now
			result.readHeader();
			return result;
		}
		public static BufferFile InitializeBufferFileInStream(System.IO.Stream fromfile, int buffersize) 
		{
			return InitializeBufferFileInStream(fromfile, buffersize, (long)0);
		}
		public static BufferFile InitializeBufferFileInStream(System.IO.Stream fromfile, int buffersize, long StartSeek) 
		{
			BufferFile result = new BufferFile(fromfile, buffersize, StartSeek);
			result.setHeader();
			return result;
		}
		void sanityCheck() 
		{
			if (this.buffersize<MINBUFFERSIZE) 
			{
				throw new BufferFileException("buffer size too small "+this.buffersize);
			}
			if (this.seekStart<0) 
			{
				throw new BufferFileException("can't start at negative position "+this.seekStart);
			}
		}
		public void getBuffer(long buffernumber, byte[] toArray, int startingAt, int length)
		{
			if (buffernumber>=this.nextBufferNumber()) 
			{
				throw new BufferFileException("last buffer is "+this.nextBufferNumber()+" not "+buffernumber);
			}
			if (length>this.buffersize) 
			{
				throw new BufferFileException("buffer size too small for retrieval "+buffersize+" need "+length);
			}
			long seekPosition = this.bufferSeek(buffernumber);
			this.fromfile.Seek(seekPosition, System.IO.SeekOrigin.Begin);
			this.fromfile.Read(toArray, startingAt, length);
		} 
		public void setBuffer(long buffernumber, byte[] fromArray, int startingAt, int length)
		{
			//System.Diagnostics.Debug.WriteLine("<br> setting buffer "+buffernumber);
			if (length>this.buffersize) 
			{
				throw new BufferFileException("buffer size too small for assignment "+buffersize+" need "+length);
			}
			if (buffernumber>this.nextBufferNumber()) 
			{
				throw new BufferFileException("cannot skip buffer numbers from "+this.nextBufferNumber()+" to "+buffernumber);
			}
			long seekPosition = this.bufferSeek(buffernumber);
			// need to fill with junk if beyond eof?
			this.fromfile.Seek(seekPosition, System.IO.SeekOrigin.Begin);
			//this.fromfile.Seek(seekPosition);
			this.fromfile.Write(fromArray, startingAt, length);
		}
		void setHeader() 
		{
			byte[] header = this.makeHeader();
			this.fromfile.Seek(this.seekStart, System.IO.SeekOrigin.Begin);
			this.fromfile.Write(header, 0, header.Length);
		}
		public void Flush() 
		{
			this.fromfile.Flush();
		}
		void readHeader() 
		{
			byte[] header = new byte[this.headersize];
			this.fromfile.Seek(this.seekStart, System.IO.SeekOrigin.Begin);
			this.fromfile.Read(header, 0, this.headersize);
			int index = 0;
			// check prefix
			foreach (byte b in HEADERPREFIX) 
			{
				if (header[index]!=b) 
				{
					throw new BufferFileException("invalid header prefix");
				}
				index++;
			}
			// skip version (for now)
			index++;
			// read buffersize
			this.buffersize = Retrieve(header, index);
			this.sanityCheck();
			//this.header = header;
		}
		public byte[] makeHeader() 
		{
			byte[] result = new byte[this.headersize];
			HEADERPREFIX.CopyTo(result, 0);
			result[HEADERPREFIX.Length] = VERSION;
			Store(this.buffersize, result, HEADERPREFIX.Length+1);
			return result;
		}
		long bufferSeek(long bufferNumber) 
		{
			if (bufferNumber<0) 
			{
				throw new BufferFileException("buffer number cannot be negative");
			}
			return this.seekStart+this.headersize+(this.buffersize*bufferNumber);
		}
		public long nextBufferNumber() 
		{
			// round up the buffer number based on the current file length
			long filelength = this.fromfile.Length;
			long bufferspace = filelength-this.headersize-this.seekStart;
			long nbuffers = bufferspace/this.buffersize;
			long remainder = bufferspace%this.buffersize;
			if (remainder>0) 
			{
				return nbuffers+1;
			}
			return nbuffers;
		}
		// there are probably libraries for this, but whatever...
		public static void Store(int TheInt, byte[] ToArray, int atIndex) 
		{
			int limit=INTSTORAGE;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = (byte) (TheInt & 0xff);
				ToArray[atIndex+i] = thebyte;
				TheInt = TheInt>>8;
			}
		}
		public static void Store(short TheShort, byte[] ToArray, int atIndex) 
		{
			int limit=SHORTSTORAGE;
			int TheInt = TheShort;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = (byte) (TheInt & 0xff);
				ToArray[atIndex+i] = thebyte;
				TheInt = TheInt>>8;
			}
		}
		public static int Retrieve(byte[] ToArray, int atIndex) 
		{
			int limit=INTSTORAGE;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			int result = 0;
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = ToArray[atIndex+limit-i-1];
				result = result << 8;
				result = result | thebyte;
			}
			return result;
		}
		public static void Store(long TheLong, byte[] ToArray, int atIndex) 
		{
			int limit=LONGSTORAGE;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = (byte) (TheLong & 0xff);
				ToArray[atIndex+i] = thebyte;
				TheLong = TheLong>>8;
			}
		}
		public static long RetrieveLong(byte[] ToArray, int atIndex) 
		{
			int limit=LONGSTORAGE;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			long result = 0;
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = ToArray[atIndex+limit-i-1];
				result = result << 8;
				result = result | thebyte;
			}
			return result;
		}
		public static short RetrieveShort(byte[] ToArray, int atIndex) 
		{
			int limit=SHORTSTORAGE;
			if (atIndex+limit>ToArray.Length) 
			{
				throw new BufferFileException("can't access beyond end of array");
			}
			int result = 0;
			for (int i=0; i<limit; i++) 
			{
				byte thebyte = ToArray[atIndex+limit-i-1];
				result = (result << 8);
				result = result | thebyte;
			}
			return (short) result;
		}
	}
	public class BufferFileException: ApplicationException 
	{
		public BufferFileException(string message): base(message) 
		{
			// do nothing extra
		}
	}
}
