using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    /// <summary>
    /// This is the base function class. Each function mus derive this class and implement at least:
    /// FunctionName: The name of the function used in the query itself
    /// TypeOfResult: The result type of the evaluated function
    /// SubstringFunc(): The constructor fills the _Parameters dictionary which defines the function parameters
    /// ExecFunc(...): Is the function itself and will containing the logic. You MUST call GraphResult result =
    /// base.ExecFunc(myParams); at the beginning to verify the parameters number and types
    /// 
    /// 
    /// </summary>
    public abstract class ABaseFunction : IGQLFunction
    {

        #region Abstract methods

        /// <summary>
        /// The ouput of a describe.
        /// </summary>
        /// <returns></returns>
        public abstract String GetDescribeOutput();

        /// <summary>
        /// This will validate the function to a working base.
        /// </summary>
        /// <param name="workingBase">The working base. Might be null for type independent function calls like CURRENTDATE().</param>
        /// <param name="typeManager"></param>
        /// <returns></returns>
        public abstract bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        #endregion

        #region Virtual methods

        /// <summary>
        /// Get the return type of this methods. Default is null - neither attribute nor function is valid on this methods.
        /// </summary>
        /// <param name="myWorkingBase"></param>
        /// <param name="myTypeManager"></param>
        /// <returns></returns>
        public virtual Type GetReturnType(IAttributeDefinition myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parameters of the function.
        /// </summary>
        protected List<ParameterValue> Parameters;

        /// <summary>
        /// The Calling object. In case of User.Friends it is the edge 'Friends'
        /// </summary>
        public Object CallingObject { get; set; }

        /// <summary>
        /// The calling TypeAttribute. In case of User.Friends it is the attribute 'Friends'
        /// </summary>
        public IAttributeDefinition CallingAttribute { get; set; }

        /// <summary>
        /// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        /// </summary>
        public IVertex CallingIVertex { get; set; }

        #endregion

        #region (public) Methods

        public ABaseFunction()
        {
            Parameters = new List<ParameterValue>();
        }

        public List<ParameterValue> GetParameters()
        {
            return Parameters;
        }

        public ParameterValue GetParameter(Int32 elementAt)
        {
            ParameterValue param;
            if (elementAt >= Parameters.Count)
            {
                param = Parameters.Last();
            }
            else
            {
                param = Parameters.ElementAt(elementAt);
            }

            return param;
        }

        public virtual FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken , params FuncParameter[] myParams)
        {
            Boolean containsVariableNumOfParams = Parameters.Exists(p => p.VariableNumOfParams);

            #region check number of parameters

            if (Parameters.Count != myParams.Length && (!containsVariableNumOfParams))
            {
                throw new FunctionParameterCountMismatchException(this.PluginName, Parameters.Count, myParams.Length);
            }
            else if (containsVariableNumOfParams && myParams.Length == 0)
            {
                throw new FunctionParameterCountMismatchException(this.PluginName, Parameters.Count, myParams.Length);
            }

            #endregion

            #region check parameter types

            Int32 definedParamCounter = 0;
            for (Int32 i = 0; i < myParams.Count(); i++)
            {
                if (Parameters[definedParamCounter].Value.GetType() != ((FuncParameter)myParams[i]).Value.GetType())
                {
                    throw new FunctionParameterTypeMismatchException(Parameters[definedParamCounter].Value.GetType(), myParams[i].GetType());
                }

                if (!Parameters[definedParamCounter].VariableNumOfParams) definedParamCounter++;
            }

            #endregion

            return null;
        }

        #endregion


        #region IPluginable Members

        public abstract string PluginName { get; }

        public abstract Dictionary<string, Type> SetableParameters { get; }

        public abstract IPluginable InitializePlugin(Dictionary<string, object> myParameters = null);

        #endregion
    }
}
