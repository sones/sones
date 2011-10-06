using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    internal class RemoteHyperEdge : ARemoteEdge, IHyperEdge
    {
        #region data

        /// <summary>
        /// The single edges that are contained within this hyper edge
        /// </summary>
        public HashSet<ISingleEdge> ContainedSingleEdges;

        #endregion

        internal RemoteHyperEdge(ServiceHyperEdgeInstance mySvcEdge, IServiceToken myServiceToken) : base(mySvcEdge, myServiceToken)
        {
            ContainedSingleEdges = new HashSet<ISingleEdge>();
            foreach(var item in mySvcEdge.SingleEdges)
            {
                ContainedSingleEdges.Add(new RemoteSingleEdge(item, myServiceToken));
            }
        }

        public override IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null)
        {
            var targetVertices = new List<IVertex>();
            foreach (var item in ContainedSingleEdges)
            {
                targetVertices.Add(item.GetSourceVertex());
            }
            return targetVertices;
        }

        public IEnumerable<ISingleEdge> GetAllEdges(PropertyHyperGraphFilter.SingleEdgeFilter myFilter = null)
        {
            return ContainedSingleEdges.ToList();
        }

        public TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction)
        {
            return myHyperEdgeFunction(ContainedSingleEdges);
        }
    }
}
