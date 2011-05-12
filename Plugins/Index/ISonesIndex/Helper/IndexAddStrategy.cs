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

namespace sones.Plugins.Index.Helper
{
    /// <summary>
    /// IndexAddStrategy defines what happens if a key already exists.
    /// 
    /// By default, the underlying type of an enum is int. 
    /// We don't need this space here, so we take byte.
    /// </summary>
    public enum IndexAddStrategy : byte
    {
        /// <summary>
        /// Replace value of existing keys
        /// </summary>
        REPLACE,
        /// <summary>
        /// Merge values of existing keys.
        /// This works in multiple value inices
        /// </summary>
        MERGE,
        /// <summary>
        /// Index key has to be unique, set fails
        /// </summary>
        UNIQUE
    }
}