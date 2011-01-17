using System;
using sones.GraphFS.Element;

namespace sones.InMemoryNonRevisioned.Element
{
    /// <summary>
    /// The single edge defines a 1-1 relation within the property hypergraph
    /// </summary>
    public sealed class SingleEdge : IEdge
    {
        public T GetProperty<T>(string myPropertyName)
        {
            throw new NotImplementedException();
        }
    }
}
