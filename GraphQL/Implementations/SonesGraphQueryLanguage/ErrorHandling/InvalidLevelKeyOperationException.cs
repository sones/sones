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

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidLevelKeyOperationException : AGraphQLException
    {
        public LevelKey LevelKeyA { get; private set; }
        public LevelKey LevelKeyB { get; private set; }
        public EdgeKey EdgeKeyA { get; private set; }
        public EdgeKey EdgeKeyB { get; private set; }
        public String Operation { get; private set; }

		/// <summary>
		/// Initializes a new instance of the InvalidLevelKeyOperationException using an edge key.
		/// </summary>
		/// <param name="myLevelKey"></param>
		/// <param name="myEdgeKey"></param>
		/// <param name="myOperation"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidLevelKeyOperationException(LevelKey myLevelKey, EdgeKey myEdgeKey, String myOperation, Exception innerException = null) : base(innerException)
        {
            LevelKeyA = myLevelKey;
            EdgeKeyA = myEdgeKey;
            Operation = myOperation;
        }

		/// <summary>
		/// Initializes a new instance of the InvalidLevelKeyOperationException using a level key.
		/// </summary>
		/// <param name="myLevelKeyA"></param>
		/// <param name="myLevelKeyB"></param>
		/// <param name="myOperation"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidLevelKeyOperationException(LevelKey myLevelKeyA, LevelKey myLevelKeyB, String myOperation, Exception innerException = null) : base(innerException)
        {
            LevelKeyA = myLevelKeyA;
            LevelKeyB = myLevelKeyB;
            Operation = myOperation;
        }

        public override string ToString()
        {
            if (LevelKeyA != null && LevelKeyB != null)
                return String.Format("Invalid Operation '{0}' {1} '{2}'", LevelKeyA, Operation, LevelKeyB);
            if (EdgeKeyA != null && EdgeKeyB != null)
                return String.Format("Invalid Operation '{0}' {1} '{2}'", EdgeKeyA, Operation, EdgeKeyB);

            return String.Format("Invalid Operation '{0}' {1} '{2}'", LevelKeyA, Operation, EdgeKeyA);
        }
    }
}
