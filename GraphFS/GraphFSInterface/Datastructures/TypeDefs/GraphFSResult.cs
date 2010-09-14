/*
 * GraphFS - GraphFSResult
 * (c) Achim Friedland, 2010
 */

namespace sones.GraphFS.DataStructures
{
    
    public enum GraphFSResult
    {
        StreamExists,
        ObjectExists,
        StreamDeleted,
        ObjectDeleted,
        NotExistant,
        Symlink,
        PathError,
    }

}
