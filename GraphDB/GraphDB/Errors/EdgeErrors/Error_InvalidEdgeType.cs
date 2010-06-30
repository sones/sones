/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidEdgeType : GraphDBEdgeError
    {
        public Type[] ExpectedEdgeTypes { get; private set; }
        public Type CurrentEdgeType { get; private set; }

        public Error_InvalidEdgeType(params Type[] myExpectedEdgeTypes)
        {
            ExpectedEdgeTypes = myExpectedEdgeTypes;
        }

        public Error_InvalidEdgeType(Type myCurrentEdgeType, params Type[] myExpectedEdgeTypes)
        {
            CurrentEdgeType = myCurrentEdgeType;
            ExpectedEdgeTypes = myExpectedEdgeTypes;
        }

        public override string ToString()
        {
            if (CurrentEdgeType != null)
                return String.Format("The edge type \"{0}\" does not match the expected type: {1}", CurrentEdgeType,
                    ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
            else
                return String.Format("Invalid edge type! Use one of the following: {0}", ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }
    }
}
