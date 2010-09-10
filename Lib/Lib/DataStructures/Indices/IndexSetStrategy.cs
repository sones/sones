/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * IndexSetStrategy
 * (c) Achim Friedland, 2009 - 2010
 */

namespace sones.Lib.DataStructures.Indices
{

    public enum IndexSetStrategy : byte
    {
        REPLACE,
        MERGE,
        /// <summary>
        /// The index entry have to be unique.
        /// Throws PandoraFSException_IndexKeyAlreadyExist if key already exist.
        /// </summary>
        UNIQUE
    }

}
