/*
 * ObjectLockTypes
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.DataStructures
{

    [Flags]
    
    public enum ObjectLockTypes
    {

        NONE,

        READERLOCK,
        READERUPGRADEABLELOCK,
        WRITERLOCK

    }

}
