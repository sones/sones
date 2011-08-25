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

using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System;

namespace sones.GraphDB.Manager.QueryPlan
{
    /// <summary>
    /// The interface for all query plan manager
    /// It's main task is to convert an expression into a queryplan
    /// </summary>
    public interface IQueryPlanManager: IManager
    {
        /// <summary>
        /// Creates a queryplan from an expression
        /// </summary>
        /// <param name="myExpression">The expression that is going to be transfered into a queryplan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current transaction token</param>
        /// <returns>A queryplan</returns>
        IQueryPlan CreateQueryPlan(IExpression myExpression, bool myIsLongRunning, Int64 myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Is the expression valid
        /// </summary>
        /// <param name="myExpression">The to be validated expression</param>
        /// <returns>True or false</returns>
        bool IsValidExpression(IExpression myExpression);
    }
}
