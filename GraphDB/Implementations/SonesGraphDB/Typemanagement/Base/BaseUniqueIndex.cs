using System;

namespace sones.GraphDB.TypeManagement.Base
{
    internal enum BaseUniqueIndex : long
    {
        BaseTypeDotName  = Int64.MinValue,
        VertexTypeDotName,
        EdgeTypeDotName,
        PropertyDotName,
        BinaryPropertyDotName,
        OutgoingEdgeDotName,
        IncomingEdgeDotName,
        IndexDotName,
    }
}
