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
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphQL.GQL.Manager.Select;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidEdgeListOperationException : AGraphQLException
    {
        private EdgeList _EdgeList1;
        private EdgeList _EdgeList2;
        private EdgeKey _EdgeKey;
        private string _Operation;

		/// <summary>
		/// Initializes a new instance of the InvalidEdgeListOperationException using an edge key.
		/// </summary>
		/// <param name="myEdgeList"></param>
		/// <param name="myEdgeKey"></param>
		/// <param name="myOperation"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidEdgeListOperationException(EdgeList myEdgeList, EdgeKey myEdgeKey, string myOperation, Exception innerException = null) : base(innerException)
        {
            _EdgeList1 = myEdgeList;
            this._EdgeKey = myEdgeKey;
            this._Operation = myOperation;
        }

		/// <summary>
		/// Initializes a new instance of the InvalidEdgeListOperationException using an edge list.
		/// </summary>
		/// <param name="myEdgeList1"></param>
		/// <param name="myEdgeList2"></param>
		/// <param name="myOperation"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidEdgeListOperationException(EdgeList myEdgeList1, EdgeList myEdgeList2, string myOperation, Exception innerException = null) : base(innerException)
        {
            _EdgeList1 = myEdgeList1;
            _EdgeList2 = myEdgeList2;
            this._Operation = myOperation;
        }

        public override string ToString()
        {
            if (_EdgeKey != null)
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeKey);
            }
            else
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeList2);
            }
        }
    }
}
