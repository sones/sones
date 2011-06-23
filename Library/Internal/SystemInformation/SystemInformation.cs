using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInformation
{
    public abstract class SystemInformation
    {
        #region Singleton

        private static SystemInformation instance = CreateInstance();

        /// <summary>
        /// Gets the system dependent instance of SystemInformation.
        /// </summary>
        public static SystemInformation Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// This method returns a new instance of SystemInformation, dependend on the current platform or compile flags.
        /// </summary>
        /// <returns></returns>
        private static SystemInformation CreateInstance()
        {
#if __MonoCS__
                return new MonoSystemInformation();
#else
            return new WindowsSystemInformation();
#endif
        }

        #endregion

        #region SystemInformation methods

        /// <summary>
        /// Returns the available free space in the given directory.
        /// </summary>
        /// <param name="myPath">The path for which free space will be calculated.</param>
        /// <returns>The available free Bytes in the given directory.</returns>
        /// <exception cref="ArgumentException">
        /// If the directory is not valid or can not be read because of missing rights.
        /// </exception>
        public abstract long GetFreeSpaceForPath(String myPath);

        /// <summary>
        /// Returns the available free space for the main memory.
        /// </summary>
        /// <returns>The available free Bytes in main memory.</returns>
        public abstract long GetAvailableMainMemory();

        /// <summary>
        /// Returns the total space for the main memory.
        /// </summary>
        /// <returns>The total space in Bytes for main memory.</returns>
        public abstract long GetTotalMainMemory();

        /// <summary>
        /// Returns the size of memory consumption for the current program.
        /// </summary>
        /// <returns>The size in Bytes this program comsumes.</returns>
        public long GetMainMemoryConsumption()
        {
            return GetTotalMainMemory() - GetAvailableMainMemory();
        }

        #endregion
    }
}
