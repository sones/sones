/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
    /// This is the base function class. Each function must derive this class and implement at least:
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
        /// This will validate the function to a working base.
        /// </summary>
        /// <param name="workingBase">The working base. Might be null for type independent function calls like CURRENTDATE().</param>
        /// <param name="typeManager"></param>
        /// <returns></returns>
        public abstract bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken);

        #endregion

        #region Virtual methods

        /// <summary>
        /// Get the return type of this methods. Default is null - neither attribute nor function is valid on this methods.
        /// </summary>
        /// <returns>The type of the result inside the resulting FuncParameter.</returns>
        public virtual Type GetReturnType()
        {
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parameters of the function.
        /// </summary>
        protected List<ParameterValue> Parameters;

        ///// <summary>
        ///// The Calling object. In case of User.Friends it is the edge 'Friends'
        ///// </summary>
        //public Object CallingObject { get; set; }

        ///// <summary>
        ///// The calling TypeAttribute. In case of User.Friends it is the attribute 'Friends'
        ///// </summary>
        //public IAttributeDefinition CallingAttribute { get; set; }

        ///// <summary>
        ///// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        ///// </summary>
        //public IVertex CallingIVertex { get; set; }

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

        public virtual FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, params FuncParameter[] myParams)
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

        #region IPluginable

        public abstract string PluginName { get; }

        public abstract string PluginShortName { get; }

        public abstract string PluginDescription { get; }

        public abstract PluginParameters<Type> SetableParameters { get; }

        public abstract IPluginable InitializePlugin(String myUniqueString, Dictionary<String,Object> myParameters = null);

        public abstract void Dispose();

        #endregion
    }
}
