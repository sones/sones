/* GraphFS - DirectoryEntryInformation
 * (c) Achim Friedland, 2009
 *  
 * An directory entry information is an object which holds all
 * information on an object within an extended directory listing.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections; 
using System.Collections.Generic;


#endregion

namespace sones.GraphFS.InternalObjects
{
  
    /// <summary>
    /// An directory entry information is an object which holds all
    /// information on an object within an extended directory listing.
    /// </summary>

    
    public struct DirectoryEntryInformation
    {

        public String           Name;
        public UInt64           Timestamp;
        public UInt64           Size;
        public HashSet<String>  Streams;

    }

}
