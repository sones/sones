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
using sones.GraphDB.TypeSystem;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using sones.Library.LanguageExtensions;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class FromUNIXDate : ABaseFunction
    {
        #region constructor

        public FromUNIXDate()
        { }

        #endregion

<<<<<<< HEAD
        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
=======
        public override string GetDescribeOutput()
        {
            return "Convert from unix datime format to DBDateTime format.";
        }

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
>>>>>>> c18174100dd22130e37f0f04b6723ee1539b73b9
        {
            if ((myWorkingBase as Type) == typeof(Int64) || (myWorkingBase as Type) == typeof(UInt64))
            {
                return true;
            }
            else if ((myWorkingBase is IAttributeDefinition) && 
                        (myWorkingBase as IAttributeDefinition).Kind == AttributeType.Property && 
                        ((myWorkingBase as IPropertyDefinition).BaseType.Name.Equals(("Int64")) || (myWorkingBase as IPropertyDefinition).BaseType.Name.Equals(("UInt64"))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, params FuncParameter[] myParams)
        {
            if (myCallingObject != null)
            {
                if (myCallingObject is Int64)
                {
                    return new FuncParameter(UNIXTimeConversionExtension.FromUnixTimeStamp((Int64)myCallingObject));
                }
                else
                {
                    throw new InvalidTypeException(myCallingObject.GetType().Name, "Int64");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region IPluginable

        public override string PluginName
        {
            get { return "sones.fromunixdate"; }
        }

        public override string PluginShortName
        {
            get { return "fromunixdate"; }
        }

        public override string PluginDescription
        {
            get { return "Convert from unix datime format to DBDateTime format."; }
        }

        public override PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new FromUNIXDate();
        }

        public override void Dispose()
        { }

        #endregion

        #region IGQLFunction

        public override Type GetReturnType()
        {
            return typeof(DateTime);
        }

        #endregion
    }
}
