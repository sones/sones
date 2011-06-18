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
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.Extensions;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.GraphDB.ErrorHandling.Type;

namespace sones.Plugins.SonesGQL.Functions
{
    /// <summary>
    /// Class to get a function which calculates the max weight of a weighted edge
    /// </summary>
    public sealed class MaxWeightFunc : ABaseFunction, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new MaxWeight function
        /// </summary>
        public MaxWeightFunc()
        { }

        #endregion

        /// <summary>
        /// Output for describe statement
        /// </summary>
        public override string GetDescribeOutput()
        {
            return "This function is valid for weighted edges and will return the maximum weight.";
        }

        /// <summary>
        /// Validates the workingBase, checks if it is valid for this function
        /// </summary>
        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myWorkingBase != null &&
                myWorkingBase is IAttributeDefinition &&
                ((IAttributeDefinition)myWorkingBase).Kind == AttributeType.OutgoingEdge;
        }

        /// <summary>
        /// Executes the function on myCallingObject
        /// </summary>
        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            var currentInnerEdgeType = ((IOutgoingEdgeDefinition)myAttributeDefinition).InnerEdgeType;

            if (myCallingObject is IHyperEdge && currentInnerEdgeType.HasProperty("Weight"))
            {
                var hyperEdge = myCallingObject as IHyperEdge;

                if (currentInnerEdgeType.HasProperty("Weight"))
                {
                    var weightProperty = currentInnerEdgeType.GetPropertyDefinition("Weight");

                    var maxWeight = hyperEdge.InvokeHyperEdgeFunc<Double>(singleEdges =>
                    {
                        return Convert.ToDouble(
                            weightProperty.GetValue(
                            singleEdges
                            .OrderByDescending(edge => weightProperty.GetValue(edge))
                            .First()));
                    });

                    return new FuncParameter(maxWeight);

                }
            }

            throw new InvalidTypeException(myCallingObject.GetType().ToString(), "Weighted IHyperEdge");
        }

        public override string PluginName
        {
            get { return"sones.maxweight"; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new MaxWeightFunc();
        }

        public void Dispose()
        { }

        public override string FunctionName
        {
            get { return "maxweight"; }
        }

        public override Type GetReturnType(IAttributeDefinition myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return typeof(Int64);
        }
    }
}
