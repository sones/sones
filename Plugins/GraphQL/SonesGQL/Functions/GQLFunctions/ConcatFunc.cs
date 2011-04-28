using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class ConcatFunc : ABaseFunction, IPluginable
    {
        public ConcatFunc()
        {
            Parameters.Add(new ParameterValue("StringPart", "", true));
        }

        public override string GetDescribeOutput()
        {
            return "This will concatenate some strings. This function can be used as type independent to concatenate string values or as type dependent to concatenate an attribute output with other strings.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase == typeof(String) || 
                ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("String")))
            {
                return true; // valid for string
            }
            else if (myWorkingBase == null)
            {
                return true; // valid without a workingBase
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            StringBuilder resString = new StringBuilder();

            if (CallingObject != null)
            {
                if (CallingObject.GetType().Name.Equals("String"))
                {
                    resString.Append(CallingObject as String);
                }
            }

            foreach (FuncParameter fp in myParams)
            {
                resString.Append(fp.Value);
            }

            return new FuncParameter(resString.ToString());
        }

        public override string PluginName
        {
            get
            {
                return "sones.concat";
            }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string,Type>();
            }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new ConcatFunc();
        }

        public override string FunctionName
        {
            get { return "concat"; }
        }
    }
}
