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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Result;

namespace sones.GraphQL.GQL.Manager.Select
{
    /// <summary>
    /// A structure to store an attribute with the related alias
    /// </summary>
    public struct GroupingValue
    {
        public readonly Dictionary<String, Object> Grouping;

        public readonly List<IVertexView> ContainedVertices;

        public GroupingValue(Dictionary<String, Object> myGrouping, IVertexView myFirstVertexView)
        {
            Grouping = myGrouping;
            ContainedVertices = new List<IVertexView> { myFirstVertexView };
        }

        public void AddVertex(IVertexView aGroupedVertexView)
        {
            ContainedVertices.Add(aGroupedVertexView);
        }

        public IVertexView GenerateVertexView()
        {
            return new VertexView(
                Grouping, 
                new Dictionary<String, IEdgeView> 
                    { 
                        {
                            "ContainedVertices", 
                            new HyperEdgeView(
                                null, 
                                new List<SingleEdgeView> (
                                    ContainedVertices.Select(
                                        _ => new SingleEdgeView(null, _)))) 
                        } 
                    });
        }
    }
}
