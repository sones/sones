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
using sones.Library.VersionedPluginManager;
using ISonesGQLFunction.Structure;
using System.Collections.Generic;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions
{
    #region IGraphQLFunctionVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IQLFunction plugin versions. 
    /// Defines the min and max version for all IQLFunction implementations which will be activated used this IQLFunction.
    /// </summary>
    public static class IGQLFunctionVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion
    
    /// <summary>
    /// The interface for all GQL functions
    /// </summary>
    public interface IGQLFunction : IPluginable
    {
        ParameterValue GetParameter(Int32 elementAt);

        List<ParameterValue> GetParameters();

        FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition, Object myCallingObject, IVertex myDBObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, params FuncParameter[] myParams);

        bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken);

        Type GetReturnType();
    }
}
