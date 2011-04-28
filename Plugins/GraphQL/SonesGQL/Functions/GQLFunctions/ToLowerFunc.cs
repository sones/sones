using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class ToLowerFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public ToLowerFunc()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Returns a copy of this attribute value converted to lowercase.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) && 
                    (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && 
                    (myWorkingBase as IPropertyDefinition).IsUserDefinedType)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (CallingObject is IAttributeDefinition)
            {
                return new FuncParameter((CallingObject as IBaseType).ToString().ToLower());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override string PluginName
        {
            get { return"sones.tolower"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new ToLowerFunc();
        }

        public override string FunctionName
        {
            get { return "tolower"; }
        }
    }
}
