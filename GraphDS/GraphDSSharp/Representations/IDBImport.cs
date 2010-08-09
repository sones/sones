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

/*
 * IDBImport
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Net.Mime;
using sones.GraphDB.Structures.Result;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// An interface to import data into the database
    /// </summary>

    public interface IDBImport
    {

        ContentType     ImportContentType { get; }

        QueryResult     ParseQueryResult(String myInput);
        DBObjectReadout ParseDBObject(String myInput);

    }

}
