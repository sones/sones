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

namespace ShortestPathAlgorithms
{
    public sealed class PathFunc : ABaseFunction
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

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
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

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            #region initialize data

            // The edge we starting of (e.g. Friends)
            var typeAttribute = CallingAttribute;

            var startNode = CallingIVertex;

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
                //TODO
                //ALT
                //return new FuncParameter(new EdgeTypePath(paths, typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(paths);
            }
            else
            {
                //TODO
                //ALT
                //return new FuncParameter(new EdgeTypePath(new HashSet<List<long>>(), typeAttribute, typeAttribute.GetDBType(dbContext.DBTypeManager)), typeAttribute);
                return new FuncParameter(new HashSet<List<long>>());
            }

            #endregion
        }

        public override string PluginName
        {
            get { return "sones.path"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new PathFunc();
        }
    }
}
