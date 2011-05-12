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

namespace sones.GraphQL.ErrorHandling
{
    public class InvalidEdgeTypeException : AGraphQLEdgeException
    {
        public Type[] ExpectedEdgeTypes { get; private set; }
        public Type CurrentEdgeType { get; private set; }

        public InvalidEdgeTypeException(params Type[] myExpectedEdgeTypes)
        {
            ExpectedEdgeTypes = myExpectedEdgeTypes;
            _msg = String.Format("Invalid edge type! Use one of the following: {0}", ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));

        }

        public InvalidEdgeTypeException(Type myCurrentEdgeType, params Type[] myExpectedEdgeTypes)
        {
            CurrentEdgeType = myCurrentEdgeType;
            ExpectedEdgeTypes = myExpectedEdgeTypes;
            _msg = String.Format("The edge type \"{0}\" does not match the expected type: {1}", CurrentEdgeType,
                   ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }

    }
}
