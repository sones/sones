using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    internal class RemoteSingleEdge : ARemoteEdge, ISingleEdge
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

        /// <summary>
        /// The target vertex
        /// </summary>
        private IVertex _TargetVertex;

        #endregion

        internal RemoteSingleEdge(ServiceSingleEdgeInstance mySvcEdgeInstance, IServiceToken myServiceToken) : base(mySvcEdgeInstance, myServiceToken)
        {
            _EdgeTypeID = mySvcEdgeInstance.TypeID;
            _EdgePropertyID = mySvcEdgeInstance.EdgePropertyID;
            _SourceVertex = new RemoteVertex(mySvcEdgeInstance.SourceVertex, _ServiceToken);
            _TargetVertex = new RemoteVertex(mySvcEdgeInstance.TargetVertex, _ServiceToken);
        }

        public IVertex GetTargetVertex()
        {
            return _TargetVertex;
        }

        public override IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null)
        {
            List<IVertex> list = new List<IVertex>();
            list.Add(_TargetVertex);
            return list;
        }
    }
}
