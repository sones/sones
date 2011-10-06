using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphQL.Result;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

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
        
        
        public  List<Tuple<ServiceVertexType, List<ServiceEdgeInstance>>> ConvertVertexTypeViewList(IEnumerable<IVertexView> myVertices)
        {
            List<Tuple<ServiceVertexType, List<ServiceEdgeInstance>>> ReturnView = new List<Tuple<ServiceVertexType, List<ServiceEdgeInstance>>>();

            foreach (var VertexView in myVertices)
            {
                //todo implement convert method
            }


            return ReturnView;
        }

        private Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>> ConvertVertexView(IVertexView myVertex)
        {
            
            Tuple<ServiceVertexInstance, List<ServiceEdgeInstance>> ReturnVertexView = new Tuple<ServiceVertexInstance,List<ServiceEdgeInstance>>(null,null);

            var type = myVertex.GetProperty<String>("Type");
            if (type.Equals("Type"))
            {
                var VertexTypeID = myVertex.GetProperty<Int64>("VertexID");
                var Vertex = this._GraphDS.GetVertexType<IVertexType>(SecToken, TransToken, ServiceRequestFactory.MakeRequestGetVertexType(VertexTypeID),
                ServiceReturnConverter.ConvertOnlyVertexType);

                //todo implement convert method

            }
            else
            {

            }


            
            var Edition = myVertex.GetProperty<String>("Edition");




            //todo implement convert method
            return ReturnVertexView;
        }

            
            

        private  List<ServiceEdgeInstance> ConvertEdgeViewList(IVertex myVertex, IEnumerable<IEdgeView> myEdges)
        {
            List<ServiceEdgeInstance> ReturnEdges = new List<ServiceEdgeInstance>();

            foreach (var Edge in myEdges)
            {
                Edge.GetProperty<Int64>("PropertyID");
                //todo implement convert method
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

                //todo implement convert method


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
