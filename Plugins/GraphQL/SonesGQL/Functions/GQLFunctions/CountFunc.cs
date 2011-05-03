using System;
using System.Collections.Generic;
using System.Linq;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class CountFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public CountFunc()
        { }

        #endregion

        #region GetDescribeOutput
        
        public override string GetDescribeOutput()
        {
            return "This will count the elements of an edge and return them as UInt64 value.";
        }
        
        #endregion

        #region validate

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) &&
                    ((myWorkingBase as IAttributeDefinition).Kind == AttributeType.IncomingEdge ||
                    (myWorkingBase as IAttributeDefinition).Kind == AttributeType.OutgoingEdge))
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

        #endregion

        #region execute

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject is IHyperEdge)
            {
                return new FuncParameter((UInt64)((IHyperEdge)myCallingObject).GetAllEdges().Count());
            }
            else if (myCallingObject is ISingleEdge)
            {
                UInt64 count = 1;
                return new FuncParameter(count);
            }
            else if (myCallingObject is IEnumerable<long>)
            {
                return new FuncParameter((UInt64)(myCallingObject as IEnumerable<long>).LongCount());
            }
            else
            {
                throw new UnknownDBException("Unexpected input for COUNT aggregate.");
            }
        }

        #endregion

        #region interface member

        public override string PluginName
        {
            get
            {
                return "sones.count";
            }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string,Type>();
            }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new CountFunc();
        }

        #endregion

        public override string FunctionName
        {
            get { return "count"; }
        }
    }
}
