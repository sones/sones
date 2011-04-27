using System;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.GraphDB.ErrorHandling.Type;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class MaxWeightFunc : ABaseFunction
    {
        #region constructor

        public MaxWeightFunc()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "This function is valid for weighted edges and will return the maximum weight.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if ((myWorkingBase is IAttributeDefinition) && 
                (((myWorkingBase as IAttributeDefinition).Kind == AttributeType.OutgoingEdge) && (myWorkingBase as IOutgoingEdgeDefinition).EdgeType.Name == "Weighted"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //TODO: implement
        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (CallingObject is IHyperEdge)
            {
                foreach (var edge in (CallingObject as IHyperEdge).GetEdges())
                {
                    //if (!edge.)
                    //{
                    //    throw new InvalidTypeException(CallingObject.GetType().ToString(), "EdgeTypeWeighted");
                    //}
                }
            }
            else if (CallingObject is ISingleEdge)
            {
 
            }
            else
            {
                throw new InvalidTypeException(CallingObject.GetType().ToString(), "IHyperEdge or ISingleEdge");
            }

            return new FuncParameter((CallingObject as IOutgoingEdgeDefinition).EdgeType.GetPropertyDefinition("Weight"));
        }

        public override string PluginName
        {
            get { return "MAXWEIGHT"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new MaxWeightFunc();
        }
    }
}
