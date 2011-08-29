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
using sones.GraphDB.Expression;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A simple optimizer which doesn't optimize anything
    /// </summary>
    public sealed class BasicLogicExpressionOptimizer : ILogicExpressionOptimizer
    {
        #region constructor

        /// <summary>
        /// Creates a new BasicLogicExpressionOptimizer 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// </summary>
        public BasicLogicExpressionOptimizer()
        {

        }

        #endregion

        #region ILogicExpressionOptimizer Members

        public IExpression OptimizeExpression(IExpression myExpression)
        {
            //return the expression itself and do not optimize anything
            return myExpression;
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "BasicLogicExpressionOptimizer"; }
        }

        public String PluginShortName
        {
            get { return "BLEOpt"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }


        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {
            return new BasicLogicExpressionOptimizer();
        }

        public void Dispose()
        { }

        #endregion
    }
}