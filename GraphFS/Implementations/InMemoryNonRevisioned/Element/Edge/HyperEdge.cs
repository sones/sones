using System;
using sones.GraphFS.Element;
using System.Collections.Generic;
using sones.Library.Internal.Definitions;

namespace sones.InMemoryNonRevisioned.Element
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : IHyperEdge
    {
        #region IHyperEdge

        public IEnumerable<ISingleEdge> GetEdges()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetTargetVertices()
        {
            throw new NotImplementedException();
        }

        public IVertex GetSourceVertex()
        {
            throw new NotImplementedException();
        }

        public T GetProperty<T>(PropertyID myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(PropertyID myPropertyID)
        {
            throw new NotImplementedException();
        }

        public ulong GetCountOfProperties()
        {
            throw new NotImplementedException();
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public ulong GetCountOfUnstructuredProperties()
        {
            throw new NotImplementedException();
        }

        public string GetComment()
        {
            throw new NotImplementedException();
        }

        public new string GetType()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
