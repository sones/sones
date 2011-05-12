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
using sones.GraphDB.Expression;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This abstract class is responsible for operators
    /// which are triggered by binary expression nodes.
    /// </summary>
    public abstract class ABinaryOperator
    {

        #region General Operator infos

        public abstract String[] Symbol { get; }
        public abstract String ContraryOperationSymbol { get; }
        public abstract BinaryOperator Label { get; }
        public abstract TypesOfOperators Type { get; }

        #endregion

        public List<String> GetAttributeList(List<String> aList)
        {

            if (aList.Count == 1)
            {
                return aList;
            }
            else
            {
                return aList.GetRange(1, aList.Count - 1);
            }

        }

        #region Get tuple based on the operator (InRange allows other tuples than + or = ...)

        public static AOperationDefinition GetValidTupleReloaded(TupleDefinition myTupleDefinition,
            IGraphDB myIGraphDB,
            SecurityToken mySecurityToken,
            TransactionToken myTransactionToken)
        {
            switch (myTupleDefinition.KindOfTuple)
            {
                case KindOfTuple.Exclusive:

                    if (myTupleDefinition.TupleElements.Count == 1)
                    {
                        return myTupleDefinition.TupleElements[0].Value as ValueDefinition;
                    }
                    else
                    {
                        return myTupleDefinition;
                    }

                default:

                    return myTupleDefinition;

            }
        }

        protected AOperationDefinition CreateTupleValue(TupleDefinition myTupleDefinition)
        {
            return myTupleDefinition;
        }

        #endregion

    }
}
