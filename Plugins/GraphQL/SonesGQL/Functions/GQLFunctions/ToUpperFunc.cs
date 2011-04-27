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
    public sealed class ToUpperFunc : ABaseFunction
    {
        #region constructor
        
        public ToUpperFunc()
        { }
        
        #endregion
        
        public override string GetDescribeOutput()
        {
            return "Returns a copy of this attribute value converted to uppercase.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if (myWorkingBase is IAttributeDefinition)
                {
                    if ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).IsUserDefinedType)
                    {
                        return false;
                    }
                    else if ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("String"))
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
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (CallingObject is String)
            {
                return new FuncParameter((CallingObject as String).ToString().ToUpper());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override string PluginName
        {
            get { return "TOUPPER"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new ToUpperFunc();
        }
    }
}
