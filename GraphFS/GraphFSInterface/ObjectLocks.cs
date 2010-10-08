/*
 * ObjectLocks
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.DataStructures
{

    [Flags]
    
    public enum ObjectLocks
    {

        NONE                =  0,

        INODE               =  1,
        OBJECTLOCATOR       =  2,

        OBJECTLOCATION      =  4 | OBJECTSTREAM,
        OBJECTSTREAM        =  8 | OBJECTEDITION,
        OBJECTEDITION       = 16 | OBJECTREVISION,
        OBJECTREVISION      = 32,

        ALL                 = INODE | OBJECTLOCATOR | OBJECTLOCATION

    }

}
