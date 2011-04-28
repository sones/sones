using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using ISonesGQLFunction.Structure;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class CurrentDateFunc : ABaseFunction
    {
        #region constructor

        public CurrentDateFunc()
        { }
        
        #endregion
        
        public override string GetDescribeOutput()
        {
            return "Returns the current date and time.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            return new FuncParameter(DateTime.Now);
        }

        public override string PluginName
        {
            get { return "sones.currentdate"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new CurrentDateFunc();
        }
    }
}
