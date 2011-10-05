using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.Result;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using System.Diagnostics;
using System.Collections;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    public sealed class Dijkstra : ABaseFunction, IPluginable
    {
        #region constructor
        /// <summary>
        /// Searches shortest paths starting from "myStart" to "myEnd".
        /// </summary>
        public Dijkstra()
        {
            Parameters.Add(new ParameterValue("EndVertex", typeof(IEnumerable<IVertex>)));
            Parameters.Add(new ParameterValue("MaxDepth", typeof(UInt64)));

        }
        #endregion

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (myWorkingBase is IAttributeDefinition)
            {
                var workingTypeAttribute = myWorkingBase as IAttributeDefinition;

                if (workingTypeAttribute.Kind == AttributeType.OutgoingEdge)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #region ExecFunc
        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition,
                                                Object myCallingObject,
                                                IVertex myStartVertex,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                Int64 myTransactionToken,
                                                params FuncParameter[] myParams)
        {
            #region initialization
            if (myStartVertex == null)
                throw new InvalidFunctionParameterException("StartVertex", "Vertex that represents the start vertex", "null");

         

            //set the start node
            var currentVertex = myStartVertex;

            if ((myParams[0].Value as IEnumerable<IVertex>).First() == null)
                throw new InvalidFunctionParameterException("EndVertex", "Set of vertices that represents the target vertices", "null");

            //set the target node
            var endVertex = (myParams[0].Value as IEnumerable<IVertex>).First();


            //set the maximum depth 




            if (myAttributeDefinition == null)
                throw new InvalidFunctionParameterException("EdgeType ", "Edge type not found", "faild");

            UInt64 maxDepth = Convert.ToUInt64(myParams[1].Value);

            if (maxDepth < 1)
                throw new InvalidFunctionParameterException("MaxDepth", "Max depth to low", maxDepth.ToString());

            var myType = myGraphDB.GetVertexType<IVertexType>(
                                                      mySecurityToken,
                                                      myTransactionToken,
                                                      new sones.GraphDB.Request.RequestGetVertexType(myAttributeDefinition.RelatedType.Name),
                                                      (statistics, type) => type);

            var myEdgeType = myAttributeDefinition.ID;


            var hasProperty = myType.GetOutgoingEdgeDefinition(myAttributeDefinition.Name).InnerEdgeType.HasProperty("Weight");


            long myPropertyID = 0;

            if (hasProperty == true)
            {
                myPropertyID = myType.GetOutgoingEdgeDefinition(myAttributeDefinition.Name).InnerEdgeType.GetPropertyDefinition("Weight").ID;
            }


            long time = DateTime.Now.Ticks;




            List<UInt64> depthBuffer = new List<UInt64>();

            List<ISingleEdge> edgeBuffer = new List<ISingleEdge>();

            List<double> distanceBuffer = new List<double>();
            List<IVertex> VertexBuffer = new List<IVertex>();




            bufferDijkstra buf = new bufferDijkstra();
            dataDijkstra lists = new dataDijkstra();



            buf.add(myStartVertex, 0, 0);
            lists.add(currentVertex, 0, 0, null, currentVertex);

                      

            
            bool endVertexFlag = false; 

            #endregion
            #region Dijkstra algorithm



            double currentVertexDistance = 0;
            ulong currentVertexDepth = 0;

            double endVertexDistance = 0;
            ulong endvertexDepth = 0;

            Stopwatch clock = new Stopwatch();
            clock.Start();

            while (buf.Count != 0)
            {
                if (currentVertexDepth < maxDepth)
                {
                    var hyperEdge = currentVertex.GetOutgoingHyperEdge(myEdgeType);

                    if (hyperEdge != null)
                    {
                        var singleEdge = hyperEdge.GetAllEdges();


                        for (int iCount = 0; iCount < singleEdge.Count(); iCount++)
                        {

                            var TargetVertex = singleEdge.ElementAt(iCount).GetTargetVertex();


                            double current_distance;

                            if (hasProperty == true)
                            {
                            
                                current_distance = Math.Abs(singleEdge.ElementAt(iCount).GetProperty<double>(myPropertyID));
                               
                            }
                            else
                            {
                                current_distance = 1;
                            }

                            var current_singleEdge = singleEdge.ElementAt(iCount);

                            var TargetVertexID = lists.getElement(TargetVertex.VertexID);
                          
                            if (TargetVertexID == null)
                            {

                                if (!endVertexFlag)
                                {



                                    buf.add(TargetVertex,
                                        current_distance + currentVertexDistance,
                                       currentVertexDepth + 1);



                                    lists.add(TargetVertex,
                                        current_distance + currentVertexDistance,
                                        currentVertexDepth + 1,
                                        current_singleEdge,
                                        currentVertex);

                                }
                                else
                                    if (endVertexDistance > currentVertexDistance + current_distance ||
                                        (endVertexDistance == currentVertexDistance + current_distance &&
                                        endvertexDepth > currentVertexDepth + 1))
                                    {



                                        buf.add(TargetVertex,
                                       current_distance + currentVertexDistance,
                                      currentVertexDepth + 1);


                                        lists.add(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);


                                    }
                            }

                            else
                            {
                                if (currentVertexDistance + current_distance < TargetVertexID.Item2)
                                {
                                    if (!endVertexFlag)
                                    {



                                        buf.set(TargetVertexID.Item2,TargetVertex,
                                       current_distance + currentVertexDistance,
                                      currentVertexDepth + 1);

                                        lists.set(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);




                                    }

                                    else
                                        if (endVertexDistance > currentVertexDistance + current_distance ||
                                        (endVertexDistance == currentVertexDistance + current_distance &&
                                        endvertexDepth > currentVertexDepth + 1))
                                        {

                                            buf.set(TargetVertexID.Item2,TargetVertex,
                                        current_distance + currentVertexDistance,
                                       currentVertexDepth + 1);

                                            lists.set(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);

                                        }
                                }
                                else  if (currentVertexDistance + current_distance == TargetVertexID.Item2 && currentVertexDepth + 1 < TargetVertexID.Item3)
                                {
                                    if (!endVertexFlag)
                                    {



                                        buf.set(TargetVertexID.Item2,TargetVertex,
                                             current_distance + currentVertexDistance,
                                            currentVertexDepth + 1);

                                        lists.set(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);



                                    }
                                    else
                                        if (endVertexDistance > currentVertexDistance + current_distance ||
                                           (endVertexDistance == currentVertexDistance + current_distance &&
                                            endvertexDepth > currentVertexDepth + 1))
                                        {



                                           buf.set(TargetVertexID.Item2,TargetVertex,
                                                current_distance + currentVertexDistance,
                                                currentVertexDepth + 1);

                                            lists.set(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);

                                        }
                                }
                            }
                     
                            
                            if (TargetVertex == endVertex)
                            {


                                endVertexFlag = true;
                                var endNode = lists.getElement(endVertex.VertexID);
                                endVertexDistance = endNode.Item2;
                                endvertexDepth = endNode.Item3;



                            }

                        }
                        
                    }
                }
                //delate from Buffer current Vertex or all
                if (currentVertex == endVertex)
                {

                    buf.Clear();

                }
                else
                {

                    buf.remove(currentVertexDistance,currentVertex.VertexID);

                }

                //Minimum distance from Buffer
                if (buf.Count != 0)
                {
 
                    var minVertex = buf.min();
                    currentVertex = minVertex.Item1;
                    currentVertexDistance = minVertex.Item2;
                    currentVertexDepth = minVertex.Item3;
                }

            }
            #endregion

            clock.Stop();


            #region create output

            edgeBuffer.Add(null);
            currentVertex = endVertex;
            while (currentVertex != myStartVertex)
            {

                var current_tuple = lists.getElement(currentVertex.VertexID);

                if (current_tuple == null)
                    throw new InvalidFunctionParameterException("MaxDepth", "Max depth to low or end node is not with start node connected, find't end node", maxDepth);

                VertexBuffer.Add(currentVertex);


                distanceBuffer.Add(current_tuple.Item2);
                depthBuffer.Add(current_tuple.Item3);
                edgeBuffer.Add(current_tuple.Item4);
                currentVertex = current_tuple.Item5;

            }
            depthBuffer.Add(0);
            distanceBuffer.Add(0);
            VertexBuffer.Add(myStartVertex);

            edgeBuffer.Add(null);



            var path = createVertexView(myPropertyID, VertexBuffer, distanceBuffer, edgeBuffer, depthBuffer);

            #endregion

            distanceBuffer = null;
            VertexBuffer = null;
            edgeBuffer = null;
            depthBuffer = null;
            buf.Clear();
            lists.Clear();
            buf = null;
            lists = null;


            return new FuncParameter(path);

        }

        #endregion

        #region Dijkstra for all shortest path

        /// <summary>
        /// Algorithm Dijkstra for find all shortest path from start Vertex to all others Vertices 
        /// </summary>
        /// <param name="myAttributeDefinition"></param>
        /// <param name="myCallingObject"></param>
        /// <param name="myStartVertex"></param>
        /// <param name="myGraphDB"></param>
        /// <param name="maxDepth"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        private Tuple<IVertex, dataDijkstra> findShortPathToAll(IAttributeDefinition myAttributeDefinition,
                                                Object myCallingObject,
                                                IVertex myStartVertex,
                                                IGraphDB myGraphDB,
                                                UInt64 maxDepth,
                                                SecurityToken mySecurityToken,
                                                Int64 myTransactionToken)
        {
            if (myStartVertex == null)
                throw new InvalidFunctionParameterException("StartVertex", "Vertex that represents the start vertex", "null");

            //set the start node
            var currentVertex = myStartVertex;

            var myType = myGraphDB.GetVertexType<IVertexType>(mySecurityToken,
                                                     myTransactionToken,
                                                     new sones.GraphDB.Request.RequestGetVertexType(myAttributeDefinition.RelatedType.Name),
                                                     (statistics, type) => type);

            var myEdgeType = myAttributeDefinition.ID;


            var hasProperty = myType.GetOutgoingEdgeDefinition(myAttributeDefinition.Name).InnerEdgeType.HasProperty("Weight");


            long myPropertyID = 0;

            if (hasProperty == true)
            {
                myPropertyID = myType.GetOutgoingEdgeDefinition(myAttributeDefinition.Name).InnerEdgeType.GetPropertyDefinition("Weight").ID;
            }


            dataDijkstra lists = new dataDijkstra();
            bufferDijkstra buffer = new bufferDijkstra();


            buffer.add(myStartVertex, 0, 0);
            lists.add(currentVertex, 0, 0, null, currentVertex);


            double currentVertexDistance = 0;
            ulong currentVertexDepth = 0;


            while (buffer.Count != 0)
            {
                if (currentVertexDepth < maxDepth)
                {
                    var hyperEdge = currentVertex.GetOutgoingHyperEdge(myEdgeType);

                    if (hyperEdge != null)
                    {
                        var singleEdge = hyperEdge.GetAllEdges();


                        for (int iCount = 0; iCount < singleEdge.Count(); iCount++)
                        {

                            var TargetVertex = singleEdge.ElementAt(iCount).GetTargetVertex();


                            double current_distance;

                            if (hasProperty == true)
                            {

                                current_distance = Math.Abs(singleEdge.ElementAt(iCount).GetProperty<double>(myPropertyID));

                            }
                            else
                            {
                                current_distance = 1;
                            }

                            var current_singleEdge = singleEdge.ElementAt(iCount);

                            var TargetVertexID = lists.getElement(TargetVertex.VertexID);

                            if (TargetVertexID == null)
                            {

                                buffer.add(TargetVertex,
                                         current_distance + currentVertexDistance,
                                        currentVertexDepth + 1);



                                lists.add(TargetVertex,
                                    current_distance + currentVertexDistance,
                                    currentVertexDepth + 1,
                                    current_singleEdge,
                                    currentVertex);
                            }
                            else
                            {
                                if (currentVertexDistance + current_distance < TargetVertexID.Item2)
                                {


                                    buffer.set(TargetVertexID.Item2,TargetVertex,
                                   current_distance + currentVertexDistance,
                                  currentVertexDepth + 1);

                                    lists.set(TargetVertex,
                                   current_distance + currentVertexDistance,
                                   currentVertexDepth + 1,
                                   current_singleEdge,
                                   currentVertex);




                                }
                                else
                                    if (currentVertexDistance + current_distance == TargetVertexID.Item2 && currentVertexDepth + 1 < TargetVertexID.Item3)
                                    {


                                        buffer.set(TargetVertexID.Item2,TargetVertex,
                                             current_distance + currentVertexDistance,
                                            currentVertexDepth + 1);

                                        lists.set(TargetVertex,
                                       current_distance + currentVertexDistance,
                                       currentVertexDepth + 1,
                                       current_singleEdge,
                                       currentVertex);



                                    }

                            }

                        }
                        //delate from Buffer Vertex and any information
                    }
                }

                    buffer.remove(currentVertexDistance,currentVertex.VertexID);

                

                    //Minimum in distance from Buffer
                    if (buffer.Count != 0)
                    {

                        var minVertex = buffer.min();
                        currentVertex = minVertex.Item1;
                        currentVertexDistance = minVertex.Item2;
                        currentVertexDepth = minVertex.Item3;
                    }
                

              

                
            }
                var result = Tuple.Create(myStartVertex, lists);

                buffer.Clear();
                lists.Clear();
                buffer = null;
                lists = null;

            return result;
        }

        #endregion

        #region create Vertex View
        private VertexView createVertexView(long myPropertyID, List<IVertex> current_vertices,
                                          List<double> current_distance, List<ISingleEdge> edge, List<UInt64> current_depth)
        {

            List<ISingleEdgeView> singleEdges = new List<ISingleEdgeView>(); ;
            Dictionary<string, Object> myPropertyList = new Dictionary<string, object>();
            Dictionary<string, Object> myPropertyTwo = new Dictionary<string, Object>();
            Dictionary<string, IEdgeView> edgeView = null;

            var currentVertex = current_vertices.Last();

            if (currentVertex != null)
            {
                var iCount = current_vertices.Count - 1;
                double dist;
                if (myPropertyID != 0)
                {
                    if (edge[iCount] != null && iCount >= 0)
                        dist = Math.Abs(edge.ElementAt(iCount).GetProperty<double>(myPropertyID));
                    else
                        dist = 0;
                }
                else
                {
                    dist = 1;
                }

                myPropertyList.Add("VertexID", currentVertex.VertexID);
                myPropertyList.Add("VertexTypeID", currentVertex.VertexTypeID);
                myPropertyList.Add("Distance", current_distance.Last());
                myPropertyList.Add("Depth", current_depth.Last());


                myPropertyTwo.Add("current Distance", dist);


                current_vertices.RemoveAt(iCount);
                current_distance.RemoveAt(iCount);
                current_depth.RemoveAt(iCount);

                if (current_vertices.Count != 0)
                {

                    singleEdges.Add(new SingleEdgeView(myPropertyTwo, createVertexView(myPropertyID, current_vertices, current_distance,
                         edge, current_depth)));
                }

            }

            if (singleEdges != null && singleEdges.Count > 0)
            {
                edgeView = new Dictionary<string, IEdgeView>();
                edgeView.Add("path", new HyperEdgeView(null, singleEdges));

            }


            return new VertexView(myPropertyList, edgeView);

        }

        #endregion

        #region IPluginable member
        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new Dijkstra();
        }

        public override string PluginShortName
        {
            get { return "dijkstra"; }
        }
        public override string PluginName
        {
            get { return "sones.dijkstra"; }
        }
        
        #endregion

        #region IGQLFunction member

        public override Type GetReturnType()
        {
            return typeof(IVertexView);
        }

        #endregion

        public override string PluginDescription
        {
            get { return "Dijkstra graph algorithm plugin"; }
        }

        public override void Dispose()
        { }

    }


}
