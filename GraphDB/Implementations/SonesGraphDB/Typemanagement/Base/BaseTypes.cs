using System;

namespace sones.GraphDB.TypeManagement.Base
{
    internal enum BaseTypes : long
    {
        BaseType       = Int64.MinValue,      //Vertextype
        VertexType     = Int64.MinValue + 1,  //Vertextype
        Attribute      = Int64.MinValue + 2,  //Vertextype
        IncomingEdge   = Int64.MinValue + 3,  //Vertextype
        OutgoingEdge   = Int64.MinValue + 4,  //Vertextype
        Property       = Int64.MinValue + 5,  //Vertextype
        Index          = Int64.MinValue + 6,  //Vertextype
        Vertex         = Int64.MinValue + 7,  //Vertextype
        BinaryProperty = Int64.MinValue + 8,  //Vertextype
        EdgeType       = Int64.MinValue + 9,  //Vertextype
        Edge           = Int64.MinValue + 10, //Edgetype
        Weighted       = Int64.MinValue + 11, //Edgetype
        Orderable      = Int64.MinValue + 12, //Edgetype

    }
}
