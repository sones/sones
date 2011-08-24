/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Linq;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using sones.Plugins.SonesGQL.Functions.ShortestPathAlgorithms.BreathFirstSearch;
using sones.Library.CollectionWrapper;

namespace sones.Plugins.SonesGQL.Functions.ShortestPathAlgorithms
{
    public sealed class PathFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public PathFunc()
        {
            Parameters.Add(new ParameterValue("TargetVertex", typeof(IEnumerable<IVertex>)));
            Parameters.Add(new ParameterValue("MaxDepth", typeof(UInt64)));
            Parameters.Add(new ParameterValue("MaxPathLength", typeof(Int64)));
            Parameters.Add(new ParameterValue("OnlyShortestPath", typeof(Boolean)));
            Parameters.Add(new ParameterValue("AllPaths", typeof(Boolean), true));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "A path algorithm. This algorithm searches the shortest, all shortest or all paths up to a given depth an path length." +
                    "Depending on the parameter 'UseBidirectionalBFS' a standard BFS algorithm or a bidirectional BFS is used.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition,
                                                Object myCallingObject,
                                                IVertex myStartVertex,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken,
                                                params FuncParameter[] myParams)
        {
            #region initialize data

            var graph = myGraphDB;
            

            // The edge we starting of (e.g. Friends)
            var typeAttribute = myAttributeDefinition;

            if (myStartVertex == null)
                throw new InvalidFunctionParameterException("StartVertex", "Vertex that represents the start vertex", "null");

            //set the start node
            var startNode = myStartVertex;

            if ((myParams[0].Value as IEnumerable<IVertex>) == null)
                throw new InvalidFunctionParameterException("TargetVertex", "Set of vertices that represents the target vertices", "null");

            //set the target node
            var targetNode = (myParams[0].Value as IEnumerable<IVertex>).First();
            

            //set the maximum depth 
            UInt64 maxDepth = Convert.ToUInt64(myParams[1].Value);

            //set the maximum path length
            UInt64 maxPathLength = Convert.ToUInt64(myParams[2].Value);

            //mark if only the shortest path should be searched
            bool onlyShortestPath = Convert.ToBoolean(myParams[3].Value);

            //mark if all paths should be searched
            bool allPaths = Convert.ToBoolean(myParams[4].Value);

            if (!onlyShortestPath && !allPaths)
            {
                allPaths = true;
            }

            bool useBidirectionalBFS = false;
            
            if(myParams.GetLength(0) == 6)
                //mark if the BidirectionalBFS should be used
                useBidirectionalBFS = Convert.ToBoolean(myParams[5].Value);

            var vertexType = myGraphDB.GetVertexType<IVertexType>(mySecurityToken, myTransactionToken, new GraphDB.Request.RequestGetVertexType(startNode.VertexTypeID), (stats, type) => type);

            if (vertexType == null)
                throw new InvalidFunctionParameterException("StartVertexType", "VertexType that represents the start vertex type not found", startNode.VertexTypeID);
            #endregion

            #region check correctness of parameters

            //check if values are correct
            if (maxDepth < 1)
                throw new InvalidFunctionParameterException("maxDepth", ">= 1", maxDepth.ToString());

            if (maxPathLength < 2)
                throw new InvalidFunctionParameterException("maxPathLength", ">= 2", maxPathLength.ToString());

            #endregion

            #region call graph function

            HashSet<List<Tuple<long, long>>> paths = null;

            //call BFS find methods
            if (useBidirectionalBFS)
                paths = new BidirectionalBFS().Find(typeAttribute, vertexType, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);
            else
                paths = new BFS().Find(typeAttribute, vertexType, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);

            #endregion

            #region create output

            if (paths != null)
            {
                #region create outputted views

                List<List<Tuple<long, long>>.Enumerator> enumerators = new List<List<Tuple<long, long>>.Enumerator>();

                foreach (var path in paths)
                {
                    enumerators.Add(path.GetEnumerator());
                }

                var view = GenerateVertexView(enumerators);

                #endregion

                return new FuncParameter(view);
            }
            else
            {
                return new FuncParameter(new VertexView(null, null));
            }

            #endregion
        }

        private VertexView GenerateVertexView(IEnumerable<List<Tuple<long, long>>.Enumerator> myEnumerators)
        {
            var enumerators = MoveNext(myEnumerators);

            Dictionary<String, Object> props = null;
            List<ISingleEdgeView> singleEdges = null;
            Dictionary<String, IEdgeView> edge = null;
            Tuple<long, long> current = null;

            foreach (var enumerator in enumerators)
            {
                if (current == null || !enumerator.Current.Equals(current))
                    current = enumerator.Current;
                else
                    continue;

                props = new Dictionary<String, Object>();

                props.Add("VertexID", current.Item2);
                props.Add("VertexTypeID", current.Item1);

                var _enums = enumerators.Where(x => x.Current.Equals(current));
                singleEdges = new List<ISingleEdgeView>();

                foreach (var _enum in _enums)
                {
                    //call next
                    var nextLevel = GenerateVertexView(new List<List<Tuple<long, long>>.Enumerator> { _enum });

                    if (nextLevel != null)
                        singleEdges.Add(new SingleEdgeView(null, nextLevel));
                }
            }

            if (singleEdges != null && singleEdges.Count > 0)
            {
                edge = new Dictionary<string, IEdgeView>();
                edge.Add("path", new HyperEdgeView(null, singleEdges));
            }

            if (props != null)
                return new VertexView(props, edge);
            else return null;
        }

        private List<List<Tuple<long, long>>.Enumerator> MoveNext(IEnumerable<List<Tuple<long, long>>.Enumerator> myEnumerator)
        {
            List<List<Tuple<long, long>>.Enumerator> current = new List<List<Tuple<long, long>>.Enumerator>();

            foreach (var enumerator in myEnumerator)
            {
                if(!enumerator.MoveNext())
                    continue;

                current.Add(enumerator);
            }

            return current;
        }

        #region IPluginable member

        public override string PluginName
        {
            get { return "sones.path"; }
        }

        public override string PluginShortName
        {
            get { return "path"; }
        }

        public override string PluginDescription
        {
            get { return "This class provides a shortest path search algorithm."; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new PathFunc();
        }

        public override string FunctionName
        {
            get { return "path"; }
        }

        public void Dispose()
        { }

        #endregion

        #region IGQLFunction member

        public override Type GetReturnType()
        {
            return typeof(IVertexView);
        }

        #endregion
    }
}
