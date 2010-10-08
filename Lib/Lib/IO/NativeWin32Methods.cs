/* 
 * NativeWin32Methods
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace sones.Lib.IO
{

    #region Flags

    /// Mainly got from http://www.pinvoke.net/default.aspx/kernel32/CreateFile.html

    [Flags]
    public enum EFileAccess : uint
    {
        /// <summary>
        ///
        /// </summary>
        GenericRead = 0x80000000,
        /// <summary>
        ///
        /// </summary>
        GenericWrite = 0x40000000,
        /// <summary>
        ///
        /// </summary>
        GenericExecute = 0x20000000,
        /// <summary>
        ///
        /// </summary>
        GenericAll = GenericRead | GenericWrite,
    }

    [Flags]
    public enum EFileShare : uint
    {
        /// <summary>
        ///
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Enables subsequent open operations on an object to request read access.
        /// Otherwise, other processes cannot open the object if they request read access.
        /// If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        Read = 0x00000001,
        /// <summary>
        /// Enables subsequent open operations on an object to request write access.
        /// Otherwise, other processes cannot open the object if they request write access.
        /// If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        Write = 0x00000002,
        /// <summary>
        /// Enables subsequent open operations on an object to request delete access.
        /// Otherwise, other processes cannot open the object if they request delete access.
        /// If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        Delete = 0x00000004
    }

    public enum ECreationDisposition : uint
    {
        /// <summary>
        /// Creates a new file. The function fails if a specified file exists.
        /// </summary>
        New = 1,
        /// <summary>
        /// Creates a new file, always.
        /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes,
        /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// Opens a file. The function fails if the file does not exist.
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// Opens a file, always.
        /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
        /// The calling process must open the file with the GENERIC_WRITE access right.
        /// </summary>
        TruncateExisting = 5
    }

    [Flags]
    public enum EFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }

    [Flags]
    public enum FileSystemRights
    {
        AppendData = 4,
        ChangePermissions = 0x40000,
        CreateDirectories = 4,
        CreateFiles = 2,
        Delete = 0x10000,
        DeleteSubdirectoriesAndFiles = 0x40,
        ExecuteFile = 0x20,
        FullControl = 0x1f01ff,
        ListDirectory = 1,
        Modify = 0x301bf,
        Read = 0x20089,
        ReadAndExecute = 0x200a9,
        ReadAttributes = 0x80,
        ReadData = 1,
        ReadExtendedAttributes = 8,
        ReadPermissions = 0x20000,
        Synchronize = 0x100000,
        TakeOwnership = 0x80000,
        Traverse = 0x20,
        Write = 0x116,
        WriteAttributes = 0x100,
        WriteData = 2,
        WriteExtendedAttributes = 0x10
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
        public const int MAX_PATH = 260;
        public const int MAX_ALTERNATE = 14;


        public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
        public string cAlternate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WIN32_FILE_ATTRIBUTE_DATA
    {
        public int dwFileAttributes; // public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
    }

    public enum GET_FILEEX_INFO_LEVELS : int
    {
        GetFileExInfoStandard = 0,
        GetFileExMaxInfoLevel = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    #endregion

    #region SafeFileHandleUnicode
    /*
    public class SafeFileHandleUnicode : SafeHandleZeroOrMinusOneIsInvalid
    {

        public SafeFileHandleUnicode()
            : base(true)
        { }

        public SafeFileHandleUnicode(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(base.handle);

        }
    }
    */
    #endregion

    internal static class NativeWin32Methods
    {

        #region File handling

        #region CreateFileW

        // Allocate a file object in the kernel, then return a handle to it.
        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern SafeFileHandle CreateFileW(
            string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            SECURITY_ATTRIBUTES lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);
        
        #endregion

        #region ReadFileW

        // Use the file handle.
        [DllImport("kernel32.dll", EntryPoint = "ReadFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ReadFileW(IntPtr handle, byte[] bytes,
           int numBytesToRead, out int numBytesRead, IntPtr overlapped_MustBeZero);
        
        #endregion

        #region DeleteFileW

        [DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteFileW(string lpFileName);
        
        #endregion

        #region GetFileAttributesExW

        [DllImport("kernel32.dll", EntryPoint = "GetFileAttributesExW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetFileAttributesExW(string lpFileName,
          GET_FILEEX_INFO_LEVELS fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);

        #endregion

        #region SearchPathW

        [DllImport("kernel32.dll", EntryPoint = "SearchPathW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern uint SearchPathW(string lpPath,
            string lpFileName,
            string lpExtension,
            int nBufferLength,
            [MarshalAs(UnmanagedType.LPTStr)]StringBuilder lpBuffer,
            out IntPtr lpFilePart);
        
        #endregion

        #region MoveFileW

        [DllImport("kernel32.dll", EntryPoint = "MoveFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool MoveFileW(String src, String dst);

        #endregion

        #region CopyFileW

        [DllImport("kernel32.dll", EntryPoint = "CopyFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CopyFileW(
          string lpExistingFileName,
          string lpNewFileName,
          [MarshalAs(UnmanagedType.Bool)]bool bFailIfExists);

        #endregion

        #endregion

        #region Directory handling

        #region CreateDirectoryW

        [DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateDirectoryW(string lpPathName, IntPtr lpSecurityAttributes);
        
        #endregion

        #region RemoveDirectoryW

        [DllImport("kernel32.dll", EntryPoint = "RemoveDirectoryW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool RemoveDirectoryW(string lpPathName);

        #endregion

        #region DeleteVolumeMountPoint

        [DllImport("kernel32.dll", EntryPoint = "DeleteVolumeMountPoint", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteVolumeMountPoint(string lpszVolumeMountPoint);
        
        #endregion

        #endregion

        #region Find

        [DllImport("kernel32.dll", EntryPoint = "FindFirstFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", EntryPoint = "FindNextFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FindNextFileW(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", EntryPoint = "FindClose", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool FindClose(IntPtr hFindFile);

        #endregion

        #region CloseHandle

        // Free the kernel's file object (close the file).
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern bool CloseHandle(IntPtr handle);
        
        #endregion

        #region GetFullPathNameW

        [DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern uint GetFullPathNameW(string lpFileName, uint nBufferLength,
           [Out] StringBuilder lpBuffer, out StringBuilder lpFilePart);

        #endregion

    }
}
