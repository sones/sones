/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;

namespace sones.GraphDB.TypeManagement.Base
{
    public enum BaseTypes : long
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
