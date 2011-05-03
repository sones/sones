using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Function.ErrorHandling;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class SubstringFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public SubstringFunc()
        {
            Parameters.Add(new ParameterValue("StartPosition", new Int32()));
            Parameters.Add(new ParameterValue("Length", new Int32()));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Retrieves a substring from the attribute value. The substring starts at a specified character position and has a specified length.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) && 
                    (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && 
                    (myWorkingBase as IPropertyDefinition).IsUserDefinedType)
                {
                    return false;
                }
                else if (myWorkingBase is Type &&  ((Type)myWorkingBase).Name.Equals("String"))
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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (!(myCallingObject is String))
            {
                throw new FunctionParameterTypeMismatchException(typeof(String), myCallingObject.GetType());
            }

            var substring = myCallingObject.ToString().Substring((Int32)myParams[0].Value, (Int32)myParams[1].Value);
                
            return new FuncParameter(substring);
        }

        public override string PluginName
        {
            get { return"sones.substring"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new SubstringFunc();
        }

        public override string FunctionName
        {
            get { return "substring"; }
        }
    }
}
