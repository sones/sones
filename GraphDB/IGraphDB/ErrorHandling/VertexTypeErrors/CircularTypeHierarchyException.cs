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

using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if a bunch of vertex types are added, but they contains a circle in the derivation hierarchy.
    /// </summary>
    public sealed class CircularTypeHierarchyException: AGraphDBTypeException
    {

        /// <summary>
        /// The list of vertex type names, that contains the circle in derivation list.
        /// </summary>
        public IEnumerable<ATypePredefinition> TypeNames { get; private set; }

        /// <summary>
        /// Creates a new instance of CircularTypeHierarchyException.
        /// </summary>
        /// <param name="myVertexTypeNames">The list of vertex type names, that contains the circle in derivation hierarchy.</param>
        public CircularTypeHierarchyException(IEnumerable<ATypePredefinition> myTypeNames, Exception innerException = null)
        {
            TypeNames = myTypeNames;

            _msg = string.Format("The following types contains a circle in the derivation hierarchy ({0})", string.Join(",", myTypeNames.Select(t => t.TypeName)));
        }

    }
}
