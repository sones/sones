/* 
 * GraphFS - AFileObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The abstract class for all file objects and virtual file objects.
    /// </summary>
    
    public abstract class AFileObject : AFSObject
    {
        public abstract Byte[] ObjectData { get; set; }
        public abstract String ContentType   { get; set; }
    }

}