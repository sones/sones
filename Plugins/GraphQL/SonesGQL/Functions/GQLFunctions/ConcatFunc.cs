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
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class ConcatFunc : ABaseFunction
    {
        public ConcatFunc()
        {
            Parameters.Add(new ParameterValue("StringPart", typeof(String), true));
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myWorkingBase == typeof(String) || 
                ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("String")))
            {
                return true; // valid for string
            }
            else if (myWorkingBase == null)
            {
                return true; // valid without a workingBase
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            StringBuilder resString = new StringBuilder();

            if (myCallingObject != null)
            {
                if (myCallingObject is string)
                {
                    resString.Append(myCallingObject as String);
                }
            }

            foreach (FuncParameter fp in myParams)
            {
                resString.Append(fp.Value.ToString());
            }

            return new FuncParameter(resString.ToString());
        }

        #region IPluginable

        public override string PluginName
        {
            get { return "sones.concat"; }
        }

        public override string PluginShortName
        {
            get { return "concat"; }
        }

        public override string PluginDescription
        {
            get { return "This will concatenate some strings. This function can be used as type independent to concatenate string values or as type dependent to concatenate an attribute output with other strings."; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new ConcatFunc();
        }

        public override void Dispose()
        { }

        #endregion

        #region IGQLFunction

        public override Type GetReturnType()
        {
            return typeof(String);
        }

        #endregion
    }
}
