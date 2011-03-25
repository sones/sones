using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IOutgoingEdgeDefinition : IAttributeDefinition
    {
        IEdgeType EdgeType { get; }

        IVertexType SourceVertexType { get; }

        IVertexType TargetVertexType { get; }
    }
}
