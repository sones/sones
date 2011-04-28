using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class Exists : ABaseFunction, IPluginable
    {
        #region constructors

        public Exists()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Return true if an DBObject contains this attribute.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (CallingObject != null)
            {
                return new FuncParameter(true);
            }
            else
            {
                return new FuncParameter(false);
            }
        }

        public override string PluginName
        {
            get { return "sones.exists"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new Exists();
        }

        public override string FunctionName
        {
            get { return "exists"; }
        }
    }
}
