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
    public sealed class MaxWeightFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public MaxWeightFunc()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "This function is valid for weighted edges and will return the maximum weight.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myWorkingBase != null &&
                myWorkingBase is IAttributeDefinition &&
                ((IAttributeDefinition)myWorkingBase).Kind == AttributeType.OutgoingEdge;
        }

        //TODO: implement
        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject is IHyperEdge)
            {
                foreach (var edge in (myCallingObject as IHyperEdge).GetAllEdges())
                {
                    //if (!edge.)
                    //{
                    //    throw new InvalidTypeException(CallingObject.GetType().ToString(), "EdgeTypeWeighted");
                    //}
                }
            }
            else if (myCallingObject is ISingleEdge)
            {
 
            }
            else
            {
                throw new InvalidTypeException(myCallingObject.GetType().ToString(), "IHyperEdge or ISingleEdge");
            }

            return new FuncParameter((myCallingObject as IOutgoingEdgeDefinition).EdgeType.GetPropertyDefinition("Weight"));
        }

        public override string PluginName
        {
            get { return"sones.maxweight"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new MaxWeightFunc();
        }

        public override string FunctionName
        {
            get { return "maxweight"; }
        }
    }
}
