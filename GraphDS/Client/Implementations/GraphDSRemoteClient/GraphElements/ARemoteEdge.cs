using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace sones.GraphDS.GraphDSRemoteClient.GraphElements
{
    internal abstract class ARemoteEdge : ARemoteGraphElement, IEdge
    {
        #region Data

        /// <summary>
        /// The edge type id
        /// </summary>
        private Int64 _EdgeTypeID;

        /// <summary>
        /// The source vertex
        /// </summary>
        private IVertex _SourceVertex;

        protected Nullable<long> _EdgePropertyID;

        #endregion


        #region Properties

        public long EdgePropertyID
        { get { return _EdgePropertyID.Value; } }

        #endregion


        #region Constructor

        internal ARemoteEdge(ServiceEdgeInstance myEdge, IServiceToken myServiceToken) : base(myServiceToken)
        {
            _EdgeTypeID = myEdge.TypeID;
            _SourceVertex = new RemoteVertex(myEdge.SourceVertex, myServiceToken);
        }

        #endregion


        #region IEdge

        public IVertex GetSourceVertex()
        {
            return _SourceVertex;
        }

        public T GetProperty<T>(long myPropertyID)
        {
            return (T)_ServiceToken.EdgeService.GetPropertyByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyID);
        }

        public IComparable GetProperty(long myPropertyID)
        {
            return (IComparable)_ServiceToken.EdgeService.GetPropertyByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _ServiceToken.EdgeService.HasPropertyByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _ServiceToken.EdgeService.GetCountOfPropertiesByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
        }

        public IEnumerable<Tuple<long, IComparable>> GetAllProperties(PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter = null)
        {
            return _ServiceToken.EdgeService.GetAllPropertiesByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this))
                .Select(x => new Tuple<long, IComparable>(x.Item1, (IComparable)x.Item2));
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            return _ServiceToken.EdgeService.GetPropertyAsStringByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            return (T)_ServiceToken.EdgeService.GetUnstructuredPropertyByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _ServiceToken.EdgeService.HasUnstructuredPropertyByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _ServiceToken.EdgeService.GetCountOfUnstructuredPropertiesByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter = null)
        {
            return _ServiceToken.EdgeService.GetAllUnstructuredPropertiesByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            return _ServiceToken.EdgeService.GetUnstructuredPropertyAsStringByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this), myPropertyName);
        }

        public abstract IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null);

        public long EdgeTypeID
        {
            get { return _EdgeTypeID; }
        }

        public IEdgeStatistics Statistics
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        #region ARemoteGraphElement

        public override long ModificationDate
        {
            get
            {
                return _ServiceToken.EdgeService.ModificationDateByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
            }
        }

        public override long CreationDate
        {
            get
            {
                return _ServiceToken.EdgeService.CreationDateByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
            }
        }

        public override string Comment
        {
            get
            {
                return _ServiceToken.EdgeService.CommentByEdgeInstance(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeInstance(this));
            }
        }

        #endregion
    }
}
