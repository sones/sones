using System;
using sones.Library.VersionedPluginManager;
using ISonesGQLFunction.Structure;
using System.Collections.Generic;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions
{
    #region IGraphQLFunctionVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IQLFunction plugin versions. 
    /// Defines the min and max version for all IQLFunction implementations which will be activated used this IQLFunction.
    /// </summary>
    public static class IGQLFunctionVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion
    
    /// <summary>
    /// The interface for all GQL functions
    /// </summary>
    public interface IGQLFunction
    {
        ParameterValue GetParameter(Int32 elementAt);

        List<ParameterValue> GetParameters();

        FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams);

        bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        Type GetReturnType(IAttributeDefinition myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        String FunctionName { get; }
    }
}
