using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.ErrorHandling.Type;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class FromUNIXDate : ABaseFunction, IPluginable
    {
        #region constructor

        public FromUNIXDate()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Convert from unix datime format to DBDateTime format.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase == typeof(Int64))
            {
                return true;
            }
            else if ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals(("Int64")))
                                                                
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject != null)
            {
                if (myCallingObject.GetType().Name.Equals("Int64"))
                {
                    return new FuncParameter(UNIXTimeConversionExtension.FromUnixTimeStamp(Convert.ToInt64(DateTime.Now)));
                }
                else
                {
                    throw new InvalidTypeException(myCallingObject.GetType().Name, "Int64");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override string PluginName
        {
            get { return "sones.fromunixdate"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new FromUNIXDate();
        }

        public override string FunctionName
        {
            get { return "fromunixdate"; }
        }
    }
}
