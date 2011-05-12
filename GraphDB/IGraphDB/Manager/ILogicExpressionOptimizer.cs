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
using sones.GraphDB.Expression;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    #region ILogicExpressionOptimizerVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ILogicExpressionOptimizer plugin versions. 
    /// Defines the min and max version for all ILogicExpressionOptimizer implementations which will be activated
    /// </summary>
    public static class ILogicExpressionOptimizerVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all logic expression optimizer
    /// </summary>
    public interface ILogicExpressionOptimizer : IPluginable
    {
        /// <summary>
        /// Optimizes a expression
        /// </summary>
        /// <param name="myExpression">The expression that is going to be optimized</param>
        /// <returns>The optimized expression</returns>
        IExpression OptimizeExpression(IExpression myExpression);
    }
}