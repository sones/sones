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
 * IJSON_IO
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphIO.JSON
{

    /// <summary>
    /// A transformation interface for user-defined objects into an
    /// application/json representation and vice versa.
    /// </summary>

    public interface IJSON_IO
    {

        /// <summary>
        /// Serialize this object as JSON
        /// </summary>
        String ToJSON();

        /// <summary>
        /// Deserialize the content of this object from the given JSON
        /// </summary>
        void FromJSON(String myJSON);

    }

}
