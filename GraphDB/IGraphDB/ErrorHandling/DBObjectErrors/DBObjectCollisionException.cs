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
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A DBObject collision occurred
    /// </summary>
    public sealed class DBObjectCollisionException : AGraphDBObjectException
    {
        public String Object { get; private set; }

        /// <summary>
        /// Creates a new DBObjectCollisionException exception
        /// </summary>
        /// <param name="myOBject">The given DBObject</param>
        public DBObjectCollisionException(String myOBject)
        {
            Object = myOBject;
            _msg = String.Format("A DBObject collision occurred. The DBObject with attributes \"{0}\" has been inserted with a VertexID that already exists!", Object);
        }     

    }
}

