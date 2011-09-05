using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphQL.Result;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    // This Converter is currently unused, because there is no plan to implement the 'Query()' method in the API
    // VertexView / EdgeView - Objects are in general for data evaluating purposes and not for the interaction with the GraphDB - the InstanceObjects 
    // of the RemoteAPI Service are not suitable and equivalent.

    public class QueryResultConverter
    {
        private IGraphDS _GraphDS;
        private SecurityToken SecToken;
        private Int64 TransToken;

        public QueryResultConverter(IGraphDS myGraphDS, SecurityToken mySecToken, Int64 myTransToken)
        {
            _GraphDS = myGraphDS;
            SecToken = mySecToken;
            TransToken = myTransToken;
        }
        
        
        public  List<Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>>> ConvertVertexViewList(IEnumerable<IVertexView> myVertices)
        {
            List<Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>>> ReturnView = new List<Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>>>();

            foreach (var VertexView in myVertices)
            {
                //todo implement convert method
            }


            return ReturnView;
        }

        private Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>> ConvertVertexView(IVertexView myVertex)
        {
            
            Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>> ReturnVertexView;
            var VertexTypeID = myVertex.GetProperty<Int64>("VertexTypeID");
            var VertexID = myVertex.GetProperty<Int64>("VertexID");
            var Edition = myVertex.GetProperty<String>("Edition");
            var Vertex = this._GraphDS.GetVertex<IVertex>(SecToken, TransToken, ServiceRequestFactory.MakeRequestGetVertex(VertexTypeID, VertexID),
                ServiceReturnConverter.ConvertOnlyVertexInstance);

            ServiceVertexInstance ReturnVertex = new ServiceVertexInstance(Vertex);

           

            List<ServiceEdgeInstance> Edges = ConvertEdgeViewList(Vertex, myVertex.GetAllEdges().Select(x => x.Item2));
            return ReturnVertexView = new Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>>(ReturnVertex, Edges);
        }

            
            

        private  List<ServiceEdgeInstance> ConvertEdgeViewList(IVertex myVertex, IEnumerable<IEdgeView> myEdges)
        {
            List<ServiceEdgeInstance> ReturnEdges = new List<ServiceEdgeInstance>();

            foreach (var Edge in myEdges)
            {
                Edge.GetProperty<Int64>("PropertyID");
            }

            return ReturnEdges;
        }

        private  ServiceEdgeInstance ConvertEdgeView(IEdgeView myEdge)
        {
            if (myEdge is ISingleEdgeView)
            {
                ServiceSingleEdgeInstance ReturnEdge = new ServiceSingleEdgeInstance();
                var EdgeInstance =  myEdge as ISingleEdgeView;

                var EdgeType = EdgeInstance.GetProperty<Int64>("EdgeTypeID");
                var EdgeID = EdgeInstance.GetProperty<Int64>("EdgeID");

                


                return ReturnEdge;
            }
            else if (myEdge is IHyperEdgeView)
            {
                ServiceHyperEdgeInstance ReturnEdge = new ServiceHyperEdgeInstance();
                var Edge = myEdge as IHyperEdgeView;

                //todo implement convert method

                return ReturnEdge;
            }



            return null;
        }

    }
}
