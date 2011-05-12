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
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB.Expression;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public static class ABinaryLogicalOperator
    {
        public static IExpressionGraph TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, BinaryOperator myOperator)
        {
            switch (myOperator)
            {
                case BinaryOperator.AND:
                    myLeftValueObject.IntersectWith(myRightValueObject);

                    break;
                case BinaryOperator.OR:
                    
                    myLeftValueObject.UnionWith(myRightValueObject);

                    break;
                case BinaryOperator.Equals:
                case BinaryOperator.GreaterOrEqualsThan:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.InRange:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:
                default:
                    throw new ArgumentOutOfRangeException("myOperator");
            }

            return myLeftValueObject;
        }
    }
}
