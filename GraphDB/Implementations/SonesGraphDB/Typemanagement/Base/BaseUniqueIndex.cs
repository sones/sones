using System;

namespace sones.GraphDB.TypeManagement.Base
{
    internal enum BaseUniqueIndex : long
    {
        BaseTypeDotID    = Int64.MinValue,
        BaseTypeDotName  = Int64.MinValue + 1,
        AttributeDotID   = Int64.MinValue + 2,
        AttributeDotName = Int64.MinValue + 3,
        IndexDotID       = Int64.MinValue + 4,
        IndexDotName     = Int64.MinValue + 5
    }
}
