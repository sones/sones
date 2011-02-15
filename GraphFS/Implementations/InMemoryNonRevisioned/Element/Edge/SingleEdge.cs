using System;
using System.Collections.Generic;
using sones.PropertyHyperGraph;

namespace sones.InMemoryNonRevisioned.Element
{
    /// <summary>
    /// The single edge defines a 1-1 relation within the property hypergraph
    /// </summary>
    public sealed class SingleEdge : ISingleEdge
    {

        #region ISingleEdge Members

        public IVertex GetTargetVertex()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEdge Members

        public IVertex GetSourceVertex()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElement Members

        public T GetProperty<T>(ulong myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(ulong myPropertyID)
        {
            throw new NotImplementedException();
        }

        public ulong GetCountOfProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, object>> GetAllProperties(Func<ulong, object, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(ulong myPropertyID)
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

        public IEnumerable<KeyValuePair<string, object>> GetAllUnstructuredProperties(Func<string, object, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public string Comment
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreationDate
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ModificationDate
        {
            get { throw new NotImplementedException(); }
        }

        public ulong TypeID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEdgeProperties Members

        public IEdgeStatistics Statistics
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
