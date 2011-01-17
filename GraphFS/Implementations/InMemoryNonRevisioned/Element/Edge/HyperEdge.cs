using System;
using sones.GraphFS.Element;

namespace sones.InMemoryNonRevisioned.Element
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : IEdge
    {
        public T GetProperty<T>(string myPropertyName)
        {
            throw new NotImplementedException();
        }
    }
}
