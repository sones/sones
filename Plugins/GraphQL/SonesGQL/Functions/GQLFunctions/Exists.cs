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
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class Exists : ABaseFunction, IPluginable
    {
        #region constructors

        public Exists()
        { }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Return true if an DBObject contains this attribute.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Library.Commons.Transaction.TransactionToken myTransactionToken)
        {
            if (myWorkingBase != null)
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
                return new FuncParameter(true);
            }
            else
            {
                return new FuncParameter(false);
            }
        }

        public override string PluginName
        {
            get { return "sones.exists"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new Exists();
        }

        public override string FunctionName
        {
            get { return "exists"; }
        }
    }
}
