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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Function.ErrorHandling;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class SubstringFunc : ABaseFunction
    {
        #region constructor

        public SubstringFunc()
        {
            Parameters.Add(new ParameterValue("StartPosition", typeof(Int32)));
            Parameters.Add(new ParameterValue("Length", typeof(Int32)));
        }

        #endregion

        public override bool ValidateWorkingBase(Object myWorkingBase, GraphDB.IGraphDB myGraphDB, Library.Commons.Security.SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                return
                    (myWorkingBase is IPropertyDefinition) &&
                    ((IPropertyDefinition)myWorkingBase).BaseType == typeof(String);
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, params FuncParameter[] myParams)
        {
            if (!(myCallingObject is String))
            {
                throw new FunctionParameterTypeMismatchException(typeof(String), myCallingObject.GetType());
            }

            var substring = myCallingObject.ToString().Substring(Convert.ToInt32(myParams[0].Value), Convert.ToInt32(myParams[1].Value));
                
            return new FuncParameter(substring);
        }

        #region IPluginable

        public override string PluginName
        {
            get { return"sones.substring"; }
        }

        public override string PluginShortName
        {
            get { return "substring"; }
        }

        public override string PluginDescription
        {
            get { return "Retrieves a substring from the attribute value. The substring starts at a specified character position and has a specified length."; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new SubstringFunc();
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
