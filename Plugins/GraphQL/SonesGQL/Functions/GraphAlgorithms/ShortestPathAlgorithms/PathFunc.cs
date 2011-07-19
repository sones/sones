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
            /// these are the starting edges and TypeAttribute.
            /// This is not the starting DBObject but just the content of the attribute defined by TypeAttribute!!!
            Parameters.Add(new ParameterValue("TargetVertex", typeof(IEnumerable<IVertex>)));
            Parameters.Add(new ParameterValue("MaxDepth", typeof(UInt64)));
            Parameters.Add(new ParameterValue("MaxPathLength", typeof(Int64)));
            Parameters.Add(new ParameterValue("OnlyShortestPath", typeof(Boolean)));
            Parameters.Add(new ParameterValue("AllPaths", typeof(Boolean)));
            Parameters.Add(new ParameterValue("UseBidirectionalBFS", typeof(Boolean)));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return @"A path algorithm. This algorithm searches the shortest, all shortest or all paths up to a given depth an path length.
                    Depending on the parameter 'UseBidirectionalBFS' a standard BFS algorithm or a bidirectional BFS is used.";
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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myStartVertex, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            #region initialize data

            // The edge we starting of (e.g. Friends)
            var typeAttribute = myAttributeDefinition;

            if (myStartVertex == null)
                throw new InvalidFunctionParameterException("StartNode", "IVertex that represents the start node", "null");

            //set the start node
            var startNode = myStartVertex;

            //set the target node
            var targetNode = (myParams[0].Value as IEnumerable<IVertex>).First();

            if (targetNode == null)
                throw new InvalidFunctionParameterException("TargetNode", "IVertex that represents the target node", "null");

            //set the maximum depth 
            byte maxDepth = Convert.ToByte(myParams[1].Value);

            //set the maximum path length
            byte maxPathLength = Convert.ToByte(myParams[2].Value);

            //mark if only the shortest path should be searched
            bool onlyShortestPath = Convert.ToBoolean(myParams[3].Value);

            //mark if all paths should be searched
            bool allPaths = Convert.ToBoolean(myParams[4].Value);

            if (!onlyShortestPath && !allPaths)
            {
                allPaths = true;
            }

            //mark if the BidirectionalBFS should be used
            bool useBidirectionalBFS = Convert.ToBoolean(myParams[5].Value);

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

            //BFS
            //if (useBidirectionalBFS)
            //    //bidirectional BFS
            //    paths = new BidirectionalBFS().Find(typeAttribute, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);
            //else
            //    paths = new BFS().Find(typeAttribute, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);

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

                //ALT
                //return new FuncParameter(new EdgeTypePath(paths, typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(view);
            }
            else
            {
                //ALT
                //return new FuncParameter(new EdgeTypePath(new HashSet<List<long>>(), typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(new VertexView(null, null));
            }

            #endregion
        }

        private VertexView GenerateVertexView(List<List<Tuple<long, long>>.Enumerator> myEnumerators)
        {
            var enumerators = MoveNext(myEnumerators);
            
            Dictionary<String, Object> props = null;
            List<ISingleEdgeView> singleEdges = new List<ISingleEdgeView>();
            Dictionary<String, IEdgeView> edge = new Dictionary<string, IEdgeView>();
            Tuple<long, long> current = null;

            foreach (var enumerator in enumerators)
            {
                if (enumerator.Current != null)
                    current = enumerator.Current;
                else
                    continue;

                if (props == null)
                {
                    props = new Dictionary<String, Object>();

                    props.Add("VertexID", current.Item2);
                    props.Add("VertexTypeID", current.Item1);
                }

                //call next
                var nextLevel = GenerateVertexView(enumerators);

                if(nextLevel != null)
                    singleEdges.Add(new SingleEdgeView(null, nextLevel));
            }

            if(singleEdges.Count > 0)
                edge.Add("path", new HyperEdgeView(null, singleEdges));

            if (props != null)
                return new VertexView(props, edge);
            else return null;
        }

        private List<List<Tuple<long, long>>.Enumerator> MoveNext(List<List<Tuple<long, long>>.Enumerator> myEnumerator)
        {
            List<List<Tuple<long, long>>.Enumerator> current = new List<List<Tuple<long, long>>.Enumerator>();

            foreach (var enumerator in myEnumerator)
            {
                enumerator.MoveNext();

                if (enumerator.Current == null)
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
    }
}
