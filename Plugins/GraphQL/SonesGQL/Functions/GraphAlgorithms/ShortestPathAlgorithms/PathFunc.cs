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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.SonesGQL.Functions;
using ISonesGQLFunction.Structure;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using ShortestPathAlgorithms.BreathFirstSearch;
using sones.GraphQL.Result;
using sones.GraphDB.Request;

namespace ShortestPathAlgorithms
{
    public sealed class PathFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public PathFunc()
        {
            /// these are the starting edges and TypeAttribute.
            /// This is not the starting DBObject but just the content of the attribute defined by TypeAttribute!!!
            Parameters.Add(new ParameterValue("TargetDBO", null));
            Parameters.Add(new ParameterValue("MaxDepth", new Int64()));
            Parameters.Add(new ParameterValue("MaxPathLength", new Int64()));
            Parameters.Add(new ParameterValue("OnlyShortestPath", new Boolean()));
            Parameters.Add(new ParameterValue("AllPaths", new Boolean()));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "A path algorithm.";
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
                else if (workingTypeAttribute.Kind == AttributeType.Property && (workingTypeAttribute as IPropertyDefinition).IsUserDefinedType)
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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            #region initialize data

            // The edge we starting of (e.g. Friends)
            var typeAttribute = myAttributeDefinition;

            var startNode = myDBObject;

            var targetNode = (myParams[0].Value as IVertex);

            byte maxDepth = Convert.ToByte((Int64)myParams[1].Value);

            byte maxPathLength = Convert.ToByte((Int64)myParams[2].Value);

            bool onlyShortestPath = (Boolean)myParams[3].Value;

            bool allPaths = (Boolean)myParams[4].Value;

            if (!onlyShortestPath && !allPaths)
            {
                allPaths = true;
            }

            #endregion

            #region check correctness of parameters

            //check if values are correct
            if (maxDepth < 1)
            {
                throw new InvalidFunctionParameterException("maxDepth", ">= 1", maxDepth.ToString());
            }

            if (maxPathLength < 2)
            {
                throw new InvalidFunctionParameterException("maxPathLength", ">= 2", maxPathLength.ToString());
            }

            #endregion
            
            #region call graph function

            HashSet<List<long>> paths;

            //BFS
            paths = new BFS().Find(typeAttribute, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);

            //bidirectional BFS
            paths = new BidirectionalBFS().Find(typeAttribute, startNode, targetNode, onlyShortestPath, allPaths, maxDepth, maxPathLength);
                                

            if (paths != null)
            {
                #region create outputted vertexviews

                var props = new Dictionary<String, object>();                
                var edges = new Dictionary<String, IEdgeView>();

                foreach (var vertex in paths)
                {
                    #region get properties
                    #endregion
                }

                var vertexView = new VertexView(props, edges);

                #endregion

                
                //ALT
                //return new FuncParameter(new EdgeTypePath(paths, typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(paths);
            }
            else
            {
                //ALT
                //return new FuncParameter(new EdgeTypePath(new HashSet<List<long>>(), typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(new HashSet<List<long>>());
            }

            #endregion
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
        {}

        #endregion
    }
}
