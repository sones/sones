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
using sones.GraphDB.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Function.ErrorHandling;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class InsertFunc : ABaseFunction, IPluginable
    {
        #region constructor

        public InsertFunc()
        {
            Parameters.Add(new ParameterValue("Position", typeof(Int32)));
            Parameters.Add(new ParameterValue("StringPart", typeof(String), true));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "This function inserts one or more strings at the given position.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (myWorkingBase != null)
            {
                if ((myWorkingBase is IAttributeDefinition) &&
                        (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property &&
                        ((myWorkingBase as IPropertyDefinition).IsUserDefinedType))
                {
                    return false;
                }
                else if ((myWorkingBase is IAttributeDefinition) &&
                        (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property &&
                        ((myWorkingBase as IPropertyDefinition).BaseType.Name.Equals("String")))
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

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, params FuncParameter[] myParams)
        {
            if (!(myCallingObject is String))
            {
                throw new FunctionParameterTypeMismatchException(typeof(String), myCallingObject.GetType());
            }

            var pos = Convert.ToInt32(myParams[0].Value);

            StringBuilder resString = new StringBuilder();
            bool dontInsert = false;

            if (pos > (myCallingObject as String).Length)
            {
                dontInsert = true;
                resString.Append((myCallingObject as String).ToString());
            }
            else
            {
                resString.Append((myCallingObject as String).ToString().Substring(0, pos));
            }
            
            foreach (FuncParameter fp in myParams.Skip(1))
            {
                resString.Append(fp.Value as String);
            }

            if(!dontInsert)
                resString.Append((myCallingObject as String).ToString().Substring(pos));

            return new FuncParameter(resString.ToString());
        }

        #region IPluginable

        public override string PluginName
        {
            get { return"sones.insert"; }
        }

        public override string PluginShortName
        {
            get { return "insert"; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new InsertFunc();
        }

        public void Dispose()
        { }

        public override string FunctionName
        {
            get { return "insert"; }
        }

        #endregion

        #region IGQLFunction

        public override Type GetReturnType()
        {
            return typeof(String);
        }

        #endregion
    }
}
