/* 
 * DirectoryHandler
 * (c) Stefan Licht, 2010
 */

//#define __MonoCS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Permissions;

namespace sones.Lib.IO
{

    /// <summary>
    /// TODO: ErrorHandling, Relative paths etc.
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
    /// Remarks:
    /// Multithreaded applications and shared library code should avoid using relative path names. 
    /// The current directory state written by the SetCurrentDirectory function is stored as a global variable in each process, therefore multithreaded applications 
    /// cannot reliably use this value without possible data corruption from other threads that may also be reading or setting this value. This limitation also applies 
    /// to the SetCurrentDirectory and GetCurrentDirectory functions. The exception being when the application is guaranteed to be running in a single thread, for example 
    /// parsing file names from the command line argument string in the main thread prior to creating any additional threads. Using relative path names in multithreaded 
    /// applications or shared library code can yield unpredictable results and is not supported.
    /// </summary>
    public class DirectoryHandler
    {

        #region Create

        /// <summary>
        /// Creates a directory
        /// Absolute paths only!!!
        /// TODO: some error checking
        /// </summary>
        /// <param name="myPath">The absolute directory path to create.</param>
        /// <returns></returns>
        public static void Create(String myPath, Boolean myCreateRecursive = false)
        {

#if(__MonoCS__)

            if (myCreateRecursive)
            {
                CreatePathIfNotExists(myPath);
            }
            Directory.CreateDirectory(myPath);

#else

            myPath = PathHandler.GetFullPathInternal(myPath); // change path to unicode path

            if (myCreateRecursive)
            {
                CreatePathIfNotExists(myPath);
            }

            //new FileIOPermission(FileIOPermissionAccess.Write, new string[] { myPath }).Demand();

            #region Create the directory using the WinAPI32

            var dirCreateResult = NativeWin32Methods.CreateDirectoryW(myPath, IntPtr.Zero);
            if (!dirCreateResult)
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

            #endregion

#endif

        }

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// Absolute paths only!!!
        /// </summary>
        /// <param name="myPath">The absolute directory path to check for existence.</param>
        /// <returns></returns>
        public static Boolean Exists(String myPath)
        {

#if(__MonoCS__)

            return Directory.Exists(myPath);

#else

            if (myPath == null)
            {
                return false;
            }
            if (myPath.Length == 0)
            {
                return false;
            }

            string fullPathInternal = PathHandler.GetFullPathInternal(myPath); // change path to unicode path
            string demandDir = GetDemandDir(fullPathInternal, true);

            //new FileIOPermission(FileIOPermissionAccess.Read, new string[] { demandDir }).Demand();

            return InternalExists(fullPathInternal);

#endif

        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the specified directory and, if indicated, any subdirectories in the directory.
        /// Absolute paths only!!!
        /// </summary>
        /// <param name="myPath">The absolute directory path to delete.</param>
        /// <param name="myRecursive">True to remove all content recursive as well.</param>
        public static void Delete(String myPath, Boolean myRecursive = false)
        {
            
#if(__MonoCS__)

            Directory.Delete(myPath, myRecursive);

#else

            myPath = PathHandler.GetFullPathInternal(myPath).TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            string demandDir = GetDemandDir(myPath, !myRecursive);

            //new FileIOPermission(FileIOPermissionAccess.Write, new string[] { demandDir }).Demand();

            #region Check whether it exists

            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FileHandler.FillAttributeInfo(myPath, ref data, false, true);
            switch (errorCode)
            {
                case 0:
                    break;

                case 2:
                    throw new System.ComponentModel.Win32Exception(3);

                default:
                    throw new System.ComponentModel.Win32Exception(errorCode);

            }

            if ((data.dwFileAttributes & 0x400) != 0)
            {
                myRecursive = false;
            }

            #endregion

            DeleteHelper(myPath, myRecursive);

#endif

        }

        #endregion

        #region EnumerateFiles

        public static IEnumerable<String> EnumerateFiles(string myPath, string mySearchPattern)
        {
                    
#if(__MonoCS__)

            return Directory.EnumerateFiles(myPath, mySearchPattern);

#else

            var path = PathHandler.GetFullPathInternal(myPath);
            if (!Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            int lastWin32Error;
            var data   = new WIN32_FIND_DATA();
            var handle = NativeWin32Methods.FindFirstFileW(path + mySearchPattern, out data);

            try
            {
                #region Check FindFirstFileW handle

                if (handle.ToInt64() < 0)
                {
                    lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 2)
                    {
                        yield break; // no files found for this pattern
                    }
                    else
                    {
                        throw new System.ComponentModel.Win32Exception(lastWin32Error);
                    }
                }

                #endregion

                #region Go through each entry

                do
                {

                    yield return System.IO.Path.Combine(myPath, data.cFileName);

                }
                while (NativeWin32Methods.FindNextFileW(handle, out data));

                #endregion

                #region Check for erros & clean up

                lastWin32Error = Marshal.GetLastWin32Error();

                if (lastWin32Error != 18) //ERROR_NO_MORE_FILES
                {
                    throw new System.ComponentModel.Win32Exception(lastWin32Error);
                }

                #endregion

            }
            finally
            {
                NativeWin32Methods.FindClose(handle);
            }

#endif

        }

        #endregion

        #region CreatePathIfNotExists

        /// <summary>
        /// Get the parent path of <paramref name="myLocation"/> and create the path if it does not exist.
        /// </summary>
        /// <param name="myLocation">The location with the leading Unicode prepend to verify.</param>
        internal static void CreatePathIfNotExists(string myLocation)
        {

            myLocation = myLocation.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var lastIndexOfSeperator = myLocation.LastIndexOfAny(new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar });

            if (lastIndexOfSeperator <= 0)
            {
                return;
            }

            var path = myLocation.Remove(lastIndexOfSeperator);

            if (path.Last() == System.IO.Path.DirectorySeparatorChar || path.Last() == System.IO.Path.AltDirectorySeparatorChar) // we cut the last part but still have a seperator
            {
                throw new System.ComponentModel.Win32Exception(123);
                //throw new DirectoryNotFoundException(myLocation.Remove(0, PathHandler.UNICODE_PREPEND.Length)); // Remove leading Unicode chars
            }

            if (path.Last() == ':') // we are at the drive level
            {
                return;
            }

            if (!Exists(path))
            {

                Create(path, true);

            }

        }

        #endregion

        #region Helper

        /// <summary>
        /// The gets the attributes of <paramref name="myPath"/> and verify that it is a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool InternalExists(string myPath)
        {

            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            return (((FileHandler.FillAttributeInfo(myPath, ref data, false, true) == 0) && ((int)data.dwFileAttributes != -1)) && (((int)data.dwFileAttributes & 0x10) != 0));
        
        }

        #region DeleteHelper

        /// <summary>
        /// Mainly a copy from the internal .Net System.IO.Directory.DeleteHelper
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="recursive"></param>
        private static void DeleteHelper(string fullPath, bool recursive)
        {
            int lastWin32Error;
            Exception exception = null;

            if (recursive)
            {
                WIN32_FIND_DATA data = new WIN32_FIND_DATA();

                var handle = NativeWin32Methods.FindFirstFileW(fullPath + System.IO.Path.DirectorySeparatorChar + "*", out data);

                try
                {

                    #region Check FindFirstFileW handle

                    if (handle.ToInt64() < 0)
                    {
                        lastWin32Error = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(lastWin32Error);
                    }

                    #endregion

                    #region Go through each entry

                    do
                    {
                        if (0 != ((int)data.dwFileAttributes & 0x10))
                        {

                            #region This is a directory

                            if (!data.cFileName.Equals(".") && !data.cFileName.Equals(".."))
                            {

                                if (0 == ((int)data.dwFileAttributes & 0x400))//& FileAttributes.ReparsePoint))
                                {

                                    #region Regular directory

                                    string str = System.IO.Path.Combine(fullPath, data.cFileName);
                                    try
                                    {
                                        DeleteHelper(str, recursive);
                                    }
                                    catch (Exception exception2)
                                    {
                                        if (exception == null)
                                        {
                                            exception = exception2;
                                        }
                                    }

                                    #endregion

                                }
                                else
                                {

                                    #region VolumeMountPoint

                                    if ((data.dwReserved0 == -1610612733) && !NativeWin32Methods.DeleteVolumeMountPoint(System.IO.Path.Combine(fullPath, data.cFileName + System.IO.Path.DirectorySeparatorChar)))
                                    {
                                        lastWin32Error = Marshal.GetLastWin32Error();
                                        throw new System.ComponentModel.Win32Exception(lastWin32Error);
                                    }
                                    if (!NativeWin32Methods.RemoveDirectoryW(System.IO.Path.Combine(fullPath, data.cFileName)))
                                    {
                                        lastWin32Error = Marshal.GetLastWin32Error();
                                        throw new System.ComponentModel.Win32Exception(lastWin32Error);
                                    }

                                    #endregion

                                }

                            }

                            #endregion

                        }
                        else if (!NativeWin32Methods.DeleteFileW(System.IO.Path.Combine(fullPath, data.cFileName)))
                        {
                            lastWin32Error = Marshal.GetLastWin32Error();
                            throw new System.ComponentModel.Win32Exception(lastWin32Error);
                        }
                    }
                    while (NativeWin32Methods.FindNextFileW(handle, out data));

                    #endregion

                    #region Check for erros & clean up

                    lastWin32Error = Marshal.GetLastWin32Error();

                    if (lastWin32Error != 18) //ERROR_NO_MORE_FILES
                    {
                        throw new System.ComponentModel.Win32Exception(lastWin32Error);
                    }

                    #endregion

                }
                finally
                {
                    NativeWin32Methods.FindClose(handle);
                }

            }

            #region Remove directory

            if (!NativeWin32Methods.RemoveDirectoryW(fullPath))
            {
                lastWin32Error = Marshal.GetLastWin32Error();
                switch (lastWin32Error)
                {
                    case 2:
                        lastWin32Error = 3;
                        break;

                    case 5:
                        //throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { userPath }));
                        break;
                }
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }

            #endregion

        }
        
        #endregion

        #region GetDemandDir

        /// <summary>
        /// Mainly a copy from the internal .Net System.IO.Directory.GetDemandDir
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="thisDirOnly"></param>
        /// <returns></returns>
        private static string GetDemandDir(string fullPath, bool thisDirOnly)
        {
            if (thisDirOnly)
            {
                if (fullPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || fullPath.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
                {
                    return (fullPath + '.');
                }
                return (fullPath + System.IO.Path.DirectorySeparatorChar + '.');
            }

            if (!fullPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) && !fullPath.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
            {
                return (fullPath + System.IO.Path.DirectorySeparatorChar);
            }

            return fullPath;
        }
        
        #endregion

        #endregion
    }
}
