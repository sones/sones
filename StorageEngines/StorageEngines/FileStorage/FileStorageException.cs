using System;
using System.Collections.Generic;
using System.Text;
using sones.StorageEngines;

namespace sones.StorageEngines.FileStorage
{

    public class FileStorageException : StorageEngineException
    {
        public FileStorageException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class FileStorageException_MakeFileSystemFailed : FileStorageException
    {
        public FileStorageException_MakeFileSystemFailed(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class FileStorageException_GrowFileSystemFailed : FileStorageException
    {
        public FileStorageException_GrowFileSystemFailed(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


}
