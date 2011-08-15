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
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.GraphDB.Extensions;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.ErrorHandling;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class TopFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public TopFunc()
        {
            Parameters.Add(new ParameterValue("NumOfEntries", typeof(UInt64)));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Will return the top elements of an edge.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.OutgoingEdge)
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
            var currentInnerEdgeType = ((IOutgoingEdgeDefinition)myAttributeDefinition).InnerEdgeType;
            bool orderByWeight = false;
            IPropertyDefinition weightProperty = null;
            int numOfEntries = Convert.ToInt32(myParams[0].Value);

            if (currentInnerEdgeType.HasProperty("Weight"))
            {
                orderByWeight = true;
                weightProperty = currentInnerEdgeType.GetPropertyDefinition("Weight");
            }

            if (myCallingObject is IHyperEdge)
            {
                var hyperEdge = myCallingObject as IHyperEdge;

                if (orderByWeight)
                {
                    var topVertices = hyperEdge.InvokeHyperEdgeFunc<IEnumerable<IVertex>>(singleEdges =>
                    {
                        return singleEdges
                            .OrderByDescending(edge => weightProperty.GetValue(edge))
                            .Select(aOrderedEdge => aOrderedEdge.GetTargetVertex());
                    }).Take(numOfEntries);

                    return new FuncParameter(topVertices);

                }
                else
                {
                    return new FuncParameter(hyperEdge.GetTargetVertices().Take(numOfEntries));
                }
            }

            throw new InvalidTypeException(myCallingObject.GetType().ToString(), "IHyperEdge");
        }

        #region IPLuginbable

        public override string PluginName
        {
            get { return"sones.top"; }
        }

        public override string PluginShortName
        {
            get { return "top"; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new TopFunc();
        }

        public void Dispose()
        { }

        public override string FunctionName
        {
            get { return "top"; }
        }

        #endregion

        #region IGQLFunction

        public override Type GetReturnType()
        {
            return typeof(IEnumerable<IVertex>);
        }

        #endregion
    }
}
