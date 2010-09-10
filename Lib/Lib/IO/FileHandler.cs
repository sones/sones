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
 * FileHandler
 * (c) Stefan Licht, 2010
 */

//#define __MonoCS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using sones.Lib.IO;
using System.Security.Permissions;


namespace sones.Lib.IO
{

    /// <summary>
    /// TODO: ErrorHandling, Relative paths, add: new FileIOPermission(FileIOPermissionAccess.Write, new string[] { fullPathInternal }, false, false).Demand();
    ///
    /// 
    /// The current System.IO implementations have some limitations in their maximum path lenght.
    /// http://msdn.microsoft.com/en-us/library/aa365247%28VS.85%29.aspx#maximum_path_length
    /// 
    /// To avoid these limitations this class invokes the unicode version of all file methods like Create, Delete etc by prepend "\\?\" to the path.
    /// 
    /// Some more usefull links:
    /// http://msdn.microsoft.com/en-us/library/aa364413%28VS.85%29.aspx
    /// http://www.pinvoke.net/default.aspx/kernel32/CreateFile.html
    /// http://help.netop.com/support/errorcodes/win32_error_codes.htm
    ///
    /// 
    /// Remarks:
    /// Multithreaded applications and shared library code should avoid using relative path names. 
    /// The current directory state written by the SetCurrentDirectory function is stored as a global variable in each process, therefore multithreaded applications 
    /// cannot reliably use this value without possible data corruption from other threads that may also be reading or setting this value. This limitation also applies 
    /// to the SetCurrentDirectory and GetCurrentDirectory functions. The exception being when the application is guaranteed to be running in a single thread, for example 
    /// parsing file names from the command line argument string in the main thread prior to creating any additional threads. Using relative path names in multithreaded 
    /// applications or shared library code can yield unpredictable results and is not supported.
    /// </summary>
    public class FileHandler
    {

        #region CreateFile

        /// <summary>
        /// Creates or overwrites a file in the specified absolute path.
        /// </summary>
        /// <param name="myPath">The path and name of the file to create.</param>
        /// <returns>A System.IO.FileStream that provides read/write access to the file specified in path.</returns>
        public static FileStream CreateFile(String myPath)
        {

            return Open(myPath, FileMode.CreateNew, FileAccess.ReadWrite, myCreatePathIfNotExists: true);

        }

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="myPath">The file to check.</param>
        /// <returns>true if the caller has the required permissions and path contains the name of an existing file; otherwise, false. This method also returns false if
        ///     path is null, an invalid path, or a zero-length string. If the caller does
        ///     not have sufficient permissions to read the specified file, no exception
        ///     is thrown and the method returns false regardless of the existence of path.
        /// </returns>
        public static Boolean Exists(String myPath)
        {
            
#if(__MonoCS__)

            return File.Exists(myPath);

#else

            myPath = PathHandler.GetFullPathInternal(myPath); // change path to unicode path

            return InternalExists(myPath);

#endif

        }

        #endregion

        #region Open

        /// <summary>
        /// Opens a System.IO.FileStream on the specified path with read/write access.
        /// </summary>
        /// <param name="myPath">The file to open.</param>
        /// <param name="myFileMode">A System.IO.FileMode value that specifies whether a file is created if one
        ///     does not exist, and determines whether the contents of existing files are
        ///     retained or overwritten.</param>
        /// <param name="myFileAccess">A System.IO.FileAccess value that specifies the operations that can be performed
        ///     on the file.</param>
        /// <param name="myFileAttributes">The desired System.IO.FileAttributes, such as Hidden, ReadOnly, Normal, and
        ///     Archive.</param>
        /// <param name="myFileShare">A System.IO.FileShare value specifying the type of access other threads have
        ///     to the file.</param>
        /// <returns>A System.IO.FileStream opened in the specified mode and path, with read/write
        ///     access and not shared.
        /// </returns>
        public static FileStream Open(String myPath, FileMode myFileMode, FileAccess myFileAccess, EFileAttributes myFileAttributes = EFileAttributes.Normal, EFileShare myFileShare = EFileShare.None, Boolean myCreatePathIfNotExists = false)
        {

#if(__MonoCS__)

            #region CreatePathIfNotExists

            if (myCreatePathIfNotExists && (myFileMode == FileMode.Create || myFileMode == FileMode.CreateNew || myFileMode == FileMode.OpenOrCreate))
            {
                DirectoryHandler.CreatePathIfNotExists(myPath);
            }

            #endregion

            return File.Open(myPath, myFileMode, myFileAccess, (FileShare)myFileShare);

#else

            myPath = PathHandler.GetFullPathInternal(myPath); // change path to unicode path

            #region CreatePathIfNotExists

            if (myCreatePathIfNotExists && (myFileMode == FileMode.Create || myFileMode == FileMode.CreateNew || myFileMode == FileMode.OpenOrCreate))
            {
                DirectoryHandler.CreatePathIfNotExists(myPath);
            }

            #endregion

            #region ECreationDisposition

            ECreationDisposition creationDisposition = (myFileMode == FileMode.Append) ? ECreationDisposition.OpenAlways : (ECreationDisposition)(int)myFileMode;

            #endregion

            #region EFileAccess

            EFileAccess fileAccess = EFileAccess.GenericAll;
            switch (myFileAccess)
            {
                case FileAccess.Read:
                    fileAccess = EFileAccess.GenericRead;
                    break;
                case FileAccess.ReadWrite:
                    fileAccess = EFileAccess.GenericAll;
                    break;
                case FileAccess.Write:
                    fileAccess = EFileAccess.GenericWrite;
                    break;
                default:
                    fileAccess = EFileAccess.GenericAll;
                    break;
            }

            #endregion

            #region Create the file using the WinAPI32 and check for errors

            var fileHandle = NativeWin32Methods.CreateFileW(myPath, fileAccess, myFileShare, null, creationDisposition, myFileAttributes, IntPtr.Zero);
            
            // Check for errors
            if (fileHandle.IsInvalid)
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

            #endregion

            #region Create FileStream and seek to end if FileMode.Append

            var stream = new FileStream(fileHandle, myFileAccess, 0x1000, false);
            if (myFileMode == FileMode.Append)
            {
                stream.Seek(0, SeekOrigin.End);
            }

            #endregion

            return stream;

#endif

        }


        #endregion

        #region Move

        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        /// <param name="mySourceFileName">The name of the file to move.</param>
        /// <param name="myDestFileName">The new path for the file.</param>
        /// <returns></returns>
        public static void Move(string mySourceFileName, string myDestFileName)
        {
            
#if(__MonoCS__)

            #region CreatePathIfNotExists

            DirectoryHandler.CreatePathIfNotExists(myDestFileName);

            #endregion

            File.Move(mySourceFileName, myDestFileName);

#else

            mySourceFileName = PathHandler.GetFullPathInternal(mySourceFileName); // change path to unicode path
            myDestFileName   = PathHandler.GetFullPathInternal(myDestFileName); // change path to unicode path

            #region CreatePathIfNotExists

            DirectoryHandler.CreatePathIfNotExists(myDestFileName);

            #endregion

            if (!NativeWin32Methods.MoveFileW(mySourceFileName, myDestFileName))
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

#endif

        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the specified file. An exception is not thrown if the specified file does not exist.
        /// </summary>
        /// <param name="myPath"></param>
        /// <returns></returns>
        public static void Delete(String myPath)
        {

#if(__MonoCS__)

            File.Delete(myPath);

#else

            if (myPath == null)
            {
                throw new ArgumentNullException("myPath");
            }

            myPath = PathHandler.GetFullPathInternal(myPath); // change path to unicode path

            //string fullPathInternal = Path.GetFullPathInternal(path);
            //new FileIOPermission(FileIOPermissionAccess.Write, new string[] { fullPathInternal }, false, false).Demand();
            //if (Environment.IsWin9X() && Directory.InternalExists(fullPathInternal))
            //{
            //    throw new UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { path }));
            //}
            if (!NativeWin32Methods.DeleteFileW(myPath))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 2)
                {
                    throw new System.ComponentModel.Win32Exception(errorCode);
                }
            }

#endif

        }

        #endregion

        #region Copy

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
        /// </summary>
        /// <param name="mySourceFileName">The file to copy.</param>
        /// <param name="myDestFileName">The name of the destination file. This cannot be a directory or an existing file.</param>
        /// <param name="myOverwrite">true if the destination file can be overwritten; otherwise, false.</param>
        public static void Copy(string mySourceFileName, string myDestFileName, Boolean myOverwrite = false)
        {
            
#if(__MonoCS__)

            File.Copy(mySourceFileName, myDestFileName, myOverwrite);

#else

            mySourceFileName = PathHandler.GetFullPathInternal(mySourceFileName); // change path to unicode path
            myDestFileName   = PathHandler.GetFullPathInternal(myDestFileName); // change path to unicode path

            #region CreatePathIfNotExists

            DirectoryHandler.CreatePathIfNotExists(myDestFileName);

            #endregion

            if (!NativeWin32Methods.CopyFileW(mySourceFileName, myDestFileName, !myOverwrite))
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

#endif

        }
        
        #endregion

        #region Helper

        internal static bool InternalExists(string path)
        {
            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            return (((FileHandler.FillAttributeInfo(path, ref data, false, true) == 0) && ((int)data.dwFileAttributes != -1)) && (((int)data.dwFileAttributes & 0x10) == 0));
        }

        internal static int FillAttributeInfo(string path, ref WIN32_FILE_ATTRIBUTE_DATA data, bool tryagain, bool returnErrorOnNotFound)
        {
            int lastWin32Error = 0;

            bool flag2 = NativeWin32Methods.GetFileAttributesExW(path, (GET_FILEEX_INFO_LEVELS)0, out data);

            if (!flag2)
            {
                lastWin32Error = Marshal.GetLastWin32Error();
                //if (((lastWin32Error != 2) && (lastWin32Error != 3)) && (lastWin32Error != 0x15))
                //{
                //    return FillAttributeInfo(path, ref data, true, returnErrorOnNotFound);
                //}
                if (!returnErrorOnNotFound)
                {
                    lastWin32Error = 0;
                    data.dwFileAttributes = -1;
                }
            }
            return lastWin32Error;
        }

        private static SECURITY_ATTRIBUTES GetSecAttrs(FileShare share)
        {
            SECURITY_ATTRIBUTES structure = null;
            if ((share & FileShare.Inheritable) != FileShare.None)
            {
                structure = new SECURITY_ATTRIBUTES();
                structure.nLength = Marshal.SizeOf(structure);
                structure.bInheritHandle = true;
            }
            return structure;
        }

        #endregion

    }
}
