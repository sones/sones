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

namespace sones.Library.TypeCodes
{
    /// <summary>
    /// A type code definition to serialize and deserialize different vertex properties correct.
    /// </summary>
    public enum TypeCodeDefinition : byte
    {
        /// <summary>
        /// The type code for base types like integer or string.
        /// </summary>
        Comparable,

        /// <summary>
        /// The type code for a set.
        /// </summary>        
        SetCollection,
  
        /// <summary>
        /// The type code for a list.
        /// </summary>
        ListCollection,

        /// <summary>
        /// The type code for a user defined type.
        /// </summary>
        UserDefinedType
    }
}
