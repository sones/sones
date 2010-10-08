/*
 * sones GraphDS API - VertexGroup
 * (c) Henning Rauch, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Result
{

    /// <summary>
    /// Groups multiple vertices.
    /// </summary>
    public class VertexGroup : Vertex
    {

        public IEnumerable<Vertex> GroupedVertices { get; protected set; }

        public VertexGroup(IDictionary<String, Object> myAttributes, IEnumerable<Vertex> myGroupedVertices)
            : base (myAttributes)
        {
            GroupedVertices = myGroupedVertices;
        }

    }

}
