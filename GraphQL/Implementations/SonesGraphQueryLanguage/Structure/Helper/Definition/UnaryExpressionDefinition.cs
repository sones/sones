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
using sones.GraphQL.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class UnaryExpressionDefinition : AExpressionDefinition
    {
        private AExpressionDefinition _Expression;
        private String _OperatorSymbol;

        public UnaryExpressionDefinition(string myOperatorSymbol, AExpressionDefinition myExpression)
        {
            _OperatorSymbol = myOperatorSymbol;
            _Expression = myExpression;
        }

        public BinaryExpressionDefinition GetBinaryExpression(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            AExpressionDefinition right;
            var op = GetOperatorBySymbol(_OperatorSymbol);
            if (op == null)
            {
                throw new OperatorDoesNotExistException(_OperatorSymbol);
            }

            right = new ValueDefinition(1);

            var binExpr = new BinaryExpressionDefinition("*", _Expression, right);
            binExpr.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

            return binExpr;
        }

    }
}
