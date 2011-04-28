using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.ErrorHandling;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class InsertFunc : ABaseFunction
    {
        #region constructor

        public InsertFunc()
        {
            Parameters.Add(new ParameterValue("Position", new Int32()));
            Parameters.Add(new ParameterValue("StringPart", "", true));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "This function inserts one or more strings at the given position.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) &&
                        (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property &&
                        ((myWorkingBase as IPropertyDefinition).IsUserDefinedType))
                {
                    return false;
                }
                else if ((myWorkingBase is IAttributeDefinition) &&
                        (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property &&
                        ((myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("String")))
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
            if (!(CallingObject.GetType().Name.Equals("String")))
            {
                throw new InvalidVertexTypeException(CallingObject.GetType().ToString(), "String");
            }

            var pos = (Int32)myParams[0].Value;

            StringBuilder resString = new StringBuilder((CallingObject as String).ToString().Substring(0, pos));
            
            foreach (FuncParameter fp in myParams.Skip(1))
            {
                resString.Append(fp.Value as String);
            }

            resString.Append((CallingObject as String).ToString().Substring(pos));

            return new FuncParameter(resString.ToString());
        }

        public override string PluginName
        {
            get { return"sones.insert"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new InsertFunc();
        }
    }
}
