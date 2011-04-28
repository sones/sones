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
using sones.GraphDB.ErrorHandling.Type;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class ToUNIXDate : ABaseFunction
    {
        #region constructor

        public ToUNIXDate()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Convert the datetime value to the unix datetime format.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase == typeof(UInt64) || myWorkingBase == typeof(DateTime))
            {
                return true;
            }
            else if (myWorkingBase is IAttributeDefinition)
            {
                if ((myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("UInt64")
                 || (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("DateTime"))
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
            if (CallingObject is UInt64)
            {
                var dtValue = new DateTime(System.Convert.ToInt64((UInt64)CallingObject));
                return new FuncParameter((Int64)UNIXTimeConversionExtension.ToUnixTimeStamp(dtValue));
            }
            else if (CallingObject is DateTime)
            {
                return new FuncParameter((Int64)(UNIXTimeConversionExtension.ToUnixTimeStamp((DateTime)CallingObject)));
            }
            else
            {
                throw new InvalidTypeException(CallingObject.GetType().ToString(), "DateTime");
            }
        }

        public override string PluginName
        {
            get { return"sones.tounixdate"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override Library.VersionedPluginManager.IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new ToUNIXDate();
        }
    }
}
