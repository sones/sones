
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This is the base function class. Each function mus derive this class and implement at least:
    /// FunctionName: The name of the function used in the query itself
    /// TypeOfResult: The result type of the evaluated function
    /// SubstringFunc(): The constructor fills the _Parameters dictionary which defines the function parameters
    /// ExecFunc(...): Is the function itself and will containing the logic. You MUST call GraphResult result =
    /// base.ExecFunc(myParams); at the beginning to verify the parameters number and types
    /// </summary>
    public abstract class ABaseFunction
    {

        #region Abstract methods

        /// <summary>
        /// A unique function name. This is used in queries to call the function.
        /// </summary>
        public abstract String FunctionName { get; }

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
        public abstract bool ValidateWorkingBase(IObject myWorkingBase, DBTypeManager myTypeManager);

        #endregion

        #region Virtual methods

        /// <summary>
        /// Get the return type of this methods. Default is null - neither attribute nor function is valid on this methods.
        /// </summary>
        /// <param name="myWorkingBase"></param>
        /// <param name="myTypeManager"></param>
        /// <returns></returns>
        public virtual IObject GetReturnType(IObject myWorkingBase, DBTypeManager myTypeManager)
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
        public TypeAttribute CallingAttribute { get; set; }

        /// <summary>
        /// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        /// </summary>
        public DBObjectStream CallingDBObjectStream { get; set; }

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
            if (param.VariableNumOfParams)
            {
                return new ParameterValue(param.Name, param.DBType.Clone(param.DBType.Value), param.VariableNumOfParams);
            }
            else
            {
                return param;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myParams">The parameters which must match the _Parameters list defined in the constructor.</param>
        /// <returns>The Value of the GraphResult is of type FuncParameter. The TypeAttribute of FuncParameter will contain NOT NULL if the TypeOfResult is a DBReference or DBList</returns>
        public virtual Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {

            Boolean containsVariableNumOfParams = Parameters.Exists(p => p.VariableNumOfParams);

            #region check number of parameters

            if (Parameters.Count != myParams.Length && (!containsVariableNumOfParams))
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this, Parameters.Count, myParams.Length));
            }
            else if (containsVariableNumOfParams && myParams.Length == 0)
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this, Parameters.Count, myParams.Length));
            }

            #endregion

            #region check parameter types

            Int32 definedParamCounter = 0;
            for (Int32 i = 0; i < myParams.Count(); i++)
            {
                if (!Parameters[definedParamCounter].DBType.IsValidValue(((FuncParameter)myParams[i]).Value))
                {
                    return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(Parameters[definedParamCounter].DBType.GetType(), myParams[i].GetType()));
                }
                Parameters[definedParamCounter].DBType.SetValue(((FuncParameter)myParams[i]).Value);

                if (!Parameters[definedParamCounter].VariableNumOfParams) definedParamCounter++;
            }

            #endregion

            return new Exceptional<FuncParameter>();

        }

        #endregion

    }

}
