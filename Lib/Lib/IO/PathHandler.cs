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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.IO
{
    public class PathHandler
    {

        /// <summary>
        /// The maximum chars for one part of the path
        /// </summary>
        private const Int32 MAX_PATH_PART_CHARS = 256;

        internal const String UNICODE_PREPEND = @"\\?\";

        /// <summary>
        /// This will check for some errors and prepend the unicode identifier \\?\ to allow more than 32,767 characters
        /// </summary>
        /// <param name="myPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static String GetFullPathInternal(String myPath)
        {

            if (String.IsNullOrEmpty(myPath))
            {
                throw new ArgumentNullException("myPath");
            }

            //System.IO.Path.CheckInvalidPathChars(myPath);

            /// Remarks:
            /// Multithreaded applications and shared library code should avoid using relative path names. 
            /// The current directory state written by the SetCurrentDirectory function is stored as a global variable in each process, therefore multithreaded applications 
            /// cannot reliably use this value without possible data corruption from other threads that may also be reading or setting this value. This limitation also applies 
            /// to the SetCurrentDirectory and GetCurrentDirectory functions. The exception being when the application is guaranteed to be running in a single thread, for example 
            /// parsing file names from the command line argument string in the main thread prior to creating any additional threads. Using relative path names in multithreaded 
            /// applications or shared library code can yield unpredictable results and is not supported.
            //NativeWin32Methods.GetFullPathNameW(myPath, )

            //var lastIndexOfDirectorySeperator = myPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            //if (lastIndexOfDirectorySeperator > 0 && (myPath.Length - lastIndexOfDirectorySeperator) > MAX_PATH_PART_CHARS)
            //{
            //    throw new System.IO.PathTooLongException();
            //}

            // change path to unicode path
            if (!myPath.StartsWith(UNICODE_PREPEND))
            {
                myPath = UNICODE_PREPEND + myPath;
            }

            return myPath;

        }

    }
}
