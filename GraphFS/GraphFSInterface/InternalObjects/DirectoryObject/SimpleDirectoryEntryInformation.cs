/* GraphFS - SimpleDirectoryEntryInformation
 * (c) Daniel Kirstenpfad, 2009
 *  
 * Actually only used to get rid of eventually existing Non Serializable data structures
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
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

    
    public struct SimpleDirectoryEntryInformation
    {

        public String                   Name;
        public Int64                    Timestamp;
        public UInt64                   Size;
        public List<String>             StreamTypes;

    }

}
