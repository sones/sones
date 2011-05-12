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
using sones.Library.VersionedPluginManager;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using ISonesGQLFunction.Structure;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class CurrentDateFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public CurrentDateFunc()
        { }
        
        #endregion
        
        public override string GetDescribeOutput()
        {
            return "Returns the current date and time.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //if (myWorkingBase == null)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
            return true;
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            return new FuncParameter(DateTime.Now);
        }

        public override string PluginName
        {
            get { return "sones.currentdate"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new CurrentDateFunc();
        }

        public override string FunctionName
        {
            get { return "currentdate"; }
        }

        public override Type GetReturnType(IAttributeDefinition myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return typeof(DateTime);
        }
    }
}
