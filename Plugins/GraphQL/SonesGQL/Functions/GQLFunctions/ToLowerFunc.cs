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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject is String)
            {
                return new FuncParameter(((String)myCallingObject).ToLower());
            }
            else
            {
                throw new FunctionParameterTypeMismatchException(typeof(String), myCallingObject.GetType());
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

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new ToLowerFunc();
        }

        public override string FunctionName
        {
            get { return "tolower"; }
        }
    }
}
