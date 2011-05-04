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
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Function.ErrorHandling;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class ToUpperFunc : ABaseFunction, IPluginable
    {
        #region constructor
        
        public ToUpperFunc()
        { }
        
        #endregion
        
        public override string GetDescribeOutput()
        {
            return "Returns a copy of this attribute value converted to uppercase.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                return (myWorkingBase is IPropertyDefinition) && 
                       (myWorkingBase as IPropertyDefinition).BaseType == typeof(String);
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject is String)
            {
                return new FuncParameter((myCallingObject as String).ToString().ToUpper());
            }
            else
            {
                throw new FunctionParameterTypeMismatchException(typeof(String), myCallingObject.GetType());
            }
        }

        public override string PluginName
        {
            get { return "sones.toupper"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new ToUpperFunc();
        }

        public override string FunctionName
        {
            get { return "toupper"; }
        }
    }
}
