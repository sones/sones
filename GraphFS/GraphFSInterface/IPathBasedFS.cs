using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones
{

    /// <summary>
    /// This interface defines methods for GraphFS implementations which based on a underlying FS like NTFS, Ext3 etc.
    /// Some indices or other structures use this interface to get the right path to the FS.
    /// </summary>
    public interface IPathBasedFS
    {

        HashSet<String> StorageLocations { get; set; }
        String GetPath(ObjectLocation myObjectLocation, UInt64 myObjectCopy, String mySuffix = "");
        String GetPathForAFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, String mySuffix = "");

    }
}
