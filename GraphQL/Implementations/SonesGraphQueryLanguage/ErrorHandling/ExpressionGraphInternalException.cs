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
using System.Diagnostics;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class ExpressionGraphInternalException : AGraphQLException
    {
        public String Info { get; private set; }

		/// <summary>
		/// Initializes a new instance of the ExpressionGraphInternalException class.
		/// </summary>
		/// <param name="myInfo"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public ExpressionGraphInternalException(String myInfo, Exception innerException = null) : base(innerException)
        {
            Info = myInfo;
            _msg = String.Format("An internal ExpressionGraph error occurred: \"{0}\"\nStacktrace:\n{1}", Info, StackTrace);
        }
    }
}
