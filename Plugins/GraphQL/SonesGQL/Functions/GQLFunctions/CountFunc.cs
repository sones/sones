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
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class CountFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public CountFunc()
        { }

        #endregion

        #region GetDescribeOutput
        
        public override string GetDescribeOutput()
        {
            return "This will count the elements of an edge and return them as UInt64 value.";
        }
        
        #endregion

        #region validate

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) &&
                    ((myWorkingBase as IAttributeDefinition).Kind == AttributeType.IncomingEdge ||
                    (myWorkingBase as IAttributeDefinition).Kind == AttributeType.OutgoingEdge))
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

        #endregion

        #region execute

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject is IHyperEdge)
            {
                return new FuncParameter((UInt64)((IHyperEdge)myCallingObject).GetAllEdges().Count());
            }
            else if (myCallingObject is ISingleEdge)
            {
                UInt64 count = 1;
                return new FuncParameter(count);
            }
            else if (myCallingObject is IEnumerable<long>)
            {
                return new FuncParameter((UInt64)(myCallingObject as IEnumerable<long>).LongCount());
            }
            else
            {
                throw new UnknownDBException("Unexpected input for COUNT aggregate.");
            }
        }

        #endregion

        #region interface member

        public override string PluginName
        {
            get
            {
                return "sones.count";
            }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string,Type>();
            }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new CountFunc();
        }

        #endregion

        public override string FunctionName
        {
            get { return "count"; }
        }
    }
}
