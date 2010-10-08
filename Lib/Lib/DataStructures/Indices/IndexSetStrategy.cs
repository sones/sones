/*
 * IndexSetStrategy
 * (c) Achim Friedland, 2009 - 2010
 */

namespace sones.Lib.DataStructures.Indices
{

    public enum IndexSetStrategy : byte
    {
        REPLACE,
        MERGE,
        /// <summary>
        /// The index entry have to be unique.
        /// Throws GraphFSException_IndexKeyAlreadyExist if key already exist.
        /// </summary>
        UNIQUE
    }

}
