using System;

namespace sones.GraphDB.TypeManagement.Base
{
    internal enum BaseTypes : long
    {
        BaseType       = Int64.MinValue,
        VertexType     = Int64.MinValue + 1,
        EdgeType       = Int64.MinValue + 2,
        Attribute      = Int64.MinValue + 3,
        IncomingEdge   = Int64.MinValue + 4,
        OutgoingEdge   = Int64.MinValue + 5,
        Property       = Int64.MinValue + 6,
        Index          = Int64.MinValue + 7,
        Vertex         = Int64.MinValue + 8,
        Edge           = Int64.MinValue + 9,
        WeightedEdge   = Int64.MinValue + 10,
        BinaryProperty = Int64.MinValue + 10
    }
}
