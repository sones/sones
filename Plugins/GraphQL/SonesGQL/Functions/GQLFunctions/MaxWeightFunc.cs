using System;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    /// <summary>
    /// Class to get a function which calculates the max weight of a weighted edge
    /// </summary>
    public sealed class MaxWeightFunc : ABaseFunction, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new MaxWeight function
        /// </summary>
        public MaxWeightFunc()
        { }

        #endregion

        /// <summary>
        /// Output for describe statement
        /// </summary>
        public override string GetDescribeOutput()
        {
            return "This function is valid for weighted edges and will return the maximum weight.";
        }

        /// <summary>
        /// Validates the workingBase, checks if it is valid for this function
        /// </summary>
        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myWorkingBase != null &&
                myWorkingBase is IAttributeDefinition &&
                ((IAttributeDefinition)myWorkingBase).Kind == AttributeType.OutgoingEdge;
        }

        /// <summary>
        /// Executes the function on myCallingObject
        /// </summary>
        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            Int64 weight = 0;

            //is there a attribute weight on the edge
            if ((myAttributeDefinition is IOutgoingEdgeDefinition) && (myAttributeDefinition as IOutgoingEdgeDefinition).EdgeType.HasAttribute("Weight"))
            {
                var propID = (myAttributeDefinition as IOutgoingEdgeDefinition).EdgeType.GetAttributeDefinition("Weight").ID;

                if (myCallingObject is IHyperEdge)
                {
                    foreach (var edge in (myCallingObject as IHyperEdge).GetAllEdges())
                    {
                        if (edge.HasProperty(propID))
                        {
                            var prop = edge.GetProperty(propID);

                            if (prop.CompareTo(weight) > 0)
                                weight = Convert.ToInt64(prop);
                        }
                    }
                }
                else if (myCallingObject is ISingleEdge)
                {
                    if ((myCallingObject as ISingleEdge).HasProperty(propID))
                    {
                        var prop = (myCallingObject as ISingleEdge).GetProperty(propID);

                        if (prop.CompareTo(weight) > 0)
                            weight = Convert.ToInt64(prop);
                    }
                }
                else
                {
                    //don't throw Exception and return 0
                    //throw new InvalidTypeException(myCallingObject.GetType().ToString(), "IHyperEdge or ISingleEdge");
                }
            }

            return new FuncParameter(weight);
        }

        public override string PluginName
        {
            get { return"sones.maxweight"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new MaxWeightFunc();
        }

        public override string FunctionName
        {
            get { return "maxweight"; }
        }

        public override Type GetReturnType(IAttributeDefinition myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return typeof(Int64);
        }
    }
}
