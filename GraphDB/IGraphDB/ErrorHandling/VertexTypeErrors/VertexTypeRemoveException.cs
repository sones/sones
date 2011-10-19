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

namespace sones.GraphDB.ErrorHandling
{
    public sealed class VertexTypeRemoveException : AGraphDBException
    {
        public String VertexType { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new TypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedVertexType">The expected type</param>
        /// <param name="myCurrentVertexType">The current type</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public VertexTypeRemoveException(String myVertexType, String myInfo, Exception innerException = null) : base(innerException)
        {
            VertexType = myVertexType;
            Info = myInfo;

            _msg = String.Format("The Vertex Type {0} cannot be removed.\n\n{1}.", VertexType, Info);
        }
    }
}
